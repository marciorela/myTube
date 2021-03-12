using Microsoft.Extensions.Logging;
using myTube.Data.Repositories;
using myTube.Domain.Entities;
using myTube.Services.Youtube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Services
{
    public class CheckVideoService
    {
        private readonly ILogger<CheckVideoService> _logger;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly CanalRepository _canalRepository;
        private readonly VideoRepository _filmeRepository;
        private readonly YoutubeServices _youTubeServices;

        public CheckVideoService(
            ILogger<CheckVideoService> logger,
            UsuarioRepository usuarioRepository,
            CanalRepository canalRepository,
            VideoRepository filmeRepository,
            YoutubeServices youTubeServices
            )
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
            _canalRepository = canalRepository;
            _filmeRepository = filmeRepository;
            _youTubeServices = youTubeServices;
        }

        public async Task GetMovies()
        {
            _logger.LogInformation("Validando videos...");
            var usuarios = await _usuarioRepository.ListAtivos();
            foreach (var usuario in usuarios)
            {
                await GetMoviesByUsuario(usuario);
            }
        }

        public async Task GetMoviesByUsuario(Usuario usuario)
        {

            var canais = await _canalRepository.GetAtivos(usuario.Id);
            foreach (var canal in canais)
            {

                var ultimaBusca = canal.UltimaBusca ?? DateTime.MinValue;
                if ((DateTime.Now - ultimaBusca).TotalHours >= 8)
                {

                    // SEMPRE BUSCAR A PARTIR DAS 0:00 DO DIA ANTERIOR AO ÚLTIMO VÍDEO
                    var publishedAfter = canal.PrimeiraBusca;
                    if (canal.UltimoVideo != null)
                    {
                        publishedAfter = ((DateTime)canal.UltimoVideo).Date.AddDays(-1);
                    }

                    var maxData = canal.UltimoVideo;
                    var filmes = await _youTubeServices.GetVideosByChannelId(usuario.ApiKey, canal.YoutubeCanalId, publishedAfter);
                    foreach (var filme in filmes)
                    {

                        var filmeDB = await _filmeRepository.GetByYoutubeId(filme.Id, usuario.Id);
                        if (filmeDB == null)
                        {
                            await _filmeRepository.Add(new Filme()
                            {
                                CanalId = canal.Id,

                                YoutubeFilmeId = filme.Id,
                                DurationSecs = filme.DurationSecs,
                                PublishedAt = filme.PublishedAt,
                                ScheduledStartTime = filme.ScheduledStartTime,
                                Summary = filme.Summary,
                                Description = filme.Description,
                                ThumbnailMaxUrl = filme.ThumbnailMaxUrl,
                                ThumbnailMediumUrl = filme.ThumbnailMediumUrl,
                                ThumbnailMinUrl = filme.ThumbnailMinUrl,
                                Title = filme.Title,
                                ETag = filme.ETag ?? ""
                            });

                            try
                            {
                                maxData ??= filme.PublishedAt;

                                maxData = new DateTime(Math.Max(((DateTime)maxData).Ticks, ((DateTime)filme.PublishedAt).Ticks));
                            }
                            catch { }
                        }
                    }
                    await _canalRepository.UpdateUltimaBusca(canal, DateTime.Now, maxData);
                }
            }
        }


    }
}
