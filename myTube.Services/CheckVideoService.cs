using Microsoft.Extensions.Logging;
using myTube.Data.Repositories;
using myTube.Domain.Entities;
using myTube.Domain.Enums;
using myTube.Services.XML;
using myTube.Services.Youtube;
using myTube.Services.Youtube.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace myTube.Services
{
    public class CheckVideoService
    {
        private readonly ILogger<CheckVideoService> _logger;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly CanalRepository _canalRepository;
        private readonly VideoRepository _filmeRepository;
        private readonly LogYoutubeRepository _logYoutubeRepository;
        private readonly YoutubeServices _youTubeServices;

        public CheckVideoService(
            ILogger<CheckVideoService> logger,
            UsuarioRepository usuarioRepository,
            CanalRepository canalRepository,
            VideoRepository filmeRepository,
            LogYoutubeRepository logYoutubeRepository,
            YoutubeServices youTubeServices
        )
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
            _canalRepository = canalRepository;
            _filmeRepository = filmeRepository;
            _logYoutubeRepository = logYoutubeRepository;
            _youTubeServices = youTubeServices;
        }

        public async Task GetMovies()
        {
            _logger.LogInformation("Validando videos...");
            var usuarios = await _usuarioRepository.ListAtivos();
            foreach (var usuario in usuarios)
            {
                await VideosByUsuario(usuario);
            }
        }

        public async Task VideosByUsuario(Usuario usuario)
        {
            // DEVE BUSCAR SOMENTE OS CANAIS ATIVOS
            // QUE TENHAM PUBLICADO VÍDEOS HÁ MAIS DE 22 HORAS
            var canais = (await _canalRepository
                .GetAtivos(usuario.Id))
                .Where(x => (DateTime.Now - (x.UltimoVideo ?? DateTime.MinValue)).TotalHours >= 22)
                .ToList();
            foreach (var canal in canais)
            {
                await VideosByChannel(canal);
            }
        }

        private async Task VideosByChannel(Canal canal)
        {
            var usedQuota = await _logYoutubeRepository.GetCost(DateTime.Now);
            var ultimaBusca = canal.UltimaBusca ?? DateTime.MinValue;
            if ((DateTime.Now - ultimaBusca).TotalMinutes >= 60 && usedQuota < 9900)
            {
                _logger.LogInformation("Verificando '{canal}'. Quota: {usedQuota}", canal.Title, usedQuota);

                var publishedAfter = canal.UltimoVideo?.AddSeconds(1) ?? canal.PrimeiraBusca;
                var (videos, _) = await _youTubeServices.GetVideosByChannelId(canal.Usuario.ApiKey, canal.UsuarioId, canal.YoutubeCanalId, canal.Source, publishedAfter);

                _logger.LogInformation("Videos encontrados: {videos}", videos.Count);

                var maxData = await UpdateVideoInformation(canal, videos);

                // FAZ A BUSCA DOS VIDEOS QUE ESTÃO NO BANCO DE DADOS, MAS ESTÃO DESATUALIZADOS
                var videosOld = await _filmeRepository.GetZeroSecondsVideos(canal);
                if (videosOld.Any())
                {
                    videos.Clear();
                    videos = videosOld.Select(x => new YoutubeMovie
                    {
                        Id = x.YoutubeFilmeId,
                        ChannelId = x.CanalId.ToString(),
                        PublishedAt = x.PublishedAt
                    }).ToList();

                    var cost = await _youTubeServices.GetExtraVideosInformation(canal.Usuario.ApiKey, videos);
                    await UpdateVideoInformation(canal, videos);

                    if (cost > 0)
                    {
                        await _logYoutubeRepository.Add("VideosByChannel", cost);
                    }
                }

                await _canalRepository.UpdateUltimaBusca(canal, DateTime.Now, maxData);
            }

        }

        private async Task<DateTime?> UpdateVideoInformation(Canal canal, List<YoutubeMovie> videos)
        {

            var maxData = canal.UltimoVideo;
            foreach (var video in videos)
            {

                var filmeDB = await _filmeRepository.GetByYoutubeId(video.Id, canal.UsuarioId);
                if (filmeDB == null)
                {
                    await _filmeRepository.Add(new Filme()
                    {
                        CanalId = canal.Id,

                        YoutubeFilmeId = video.Id,
                        DurationSecs = video.DurationSecs,
                        PublishedAt = video.PublishedAt,
                        ScheduledStartTime = video.ScheduledStartTime,
                        Summary = video.Summary,
                        Description = video.Description,
                        ThumbnailMaxUrl = video.ThumbnailMaxUrl,
                        ThumbnailMediumUrl = video.ThumbnailMediumUrl,
                        ThumbnailMinUrl = video.ThumbnailMinUrl,
                        Title = video.Title,
                        ETag = video.ETag ?? "",
                        Status = video.Checked ? EStatusVideo.NaoAssistido : EStatusVideo.Cancelado
                    });

                    try
                    {
                        maxData ??= video.PublishedAt;

                        maxData = new DateTime(Math.Max(((DateTime)maxData).Ticks, ((DateTime)video.PublishedAt).Ticks));
                    }
                    catch { }
                }
                else
                {
                    if (!video.Checked)
                    {
                        filmeDB.Status = EStatusVideo.Cancelado;
                    }

                    filmeDB.DurationSecs = video.DurationSecs;
                    await _filmeRepository.Update(filmeDB);
                }
            }

            return maxData;
        }

    }

    [Serializable, XmlRoot("entry")]
    public class Entry
    {
        [XmlElement(ElementName = "ytvideoId")]
        public string VideoId { get; set; }

        [XmlElement(ElementName = "ytchannelId")]
        public string ChannelId { get; set; }

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "published")]
        public string Published { get; set; }

        [XmlElement(ElementName = "updated")]
        public string Updated { get; set; }

        public DateTime PublishedAt
        {
            get
            {
                return DateTime.Parse(Published, CultureInfo.InvariantCulture);
            }
        }
    }
}
