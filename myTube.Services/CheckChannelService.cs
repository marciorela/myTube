using Microsoft.Extensions.Logging;
using myTube.Data.Repositories;
using myTube.Domain.Entities;
using myTube.Domain.Enums;
using myTube.Services.Youtube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Services
{
    public class CheckChannelService
    {
        private readonly ILogger<CheckChannelService> _logger;
        private readonly CanalRepository _canalRepository;
        private readonly YoutubeServices _youTubeServices;

        public CheckChannelService(
            ILogger<CheckChannelService> logger,
            CanalRepository canalRepository,
            YoutubeServices youTubeServices)
        {
            _logger = logger;
            _canalRepository = canalRepository;
            _youTubeServices = youTubeServices;
        }

        public async Task ValidarCanaisPendentes()
        {
            try
            {
                _logger.LogInformation("Validando canais...");
                var canaisValidar = await _canalRepository.GetByStatus(EStatusCanal.Validar, ESource.Canal);
                await ValidarCanais(canaisValidar);

                _logger.LogInformation("Validando playlists...");
                var playlistsValidar = await _canalRepository.GetByStatus(EStatusCanal.Validar, ESource.Playlist);
                await ValidarPlaylists(playlistsValidar);

                //_logger.LogInformation("Validando canais com erro de quota...");
                //var canaisQuota = await _canalRepository.GetByStatus(EStatusCanal.QuotaExceeded);
                //await ValidarCanais(canaisQuota);
            }
            catch
            {
                // POR ENQUANTO, NADA A FAZER
                throw;
            }
        }

        private async Task ValidarCanais(IEnumerable<Canal> canais)
        {
            foreach (var canal in canais)
            {
                try
                {

                    var (info, cost) = await _youTubeServices.GetChannelInfo(canal.Usuario.ApiKey, canal.YoutubeCanalId);
                    if (info != null)
                    {
                        canal.CustomUrl = info.CustomUrl;
                        canal.Description = info.Description;
                        canal.PublishedAt = info.PublishedAt;
                        canal.ThumbnailMinUrl = info.ThumbnailMinUrl;
                        canal.ThumbnailMediumUrl = info.ThumbnailMediumUrl;
                        canal.ThumbnailMaxUrl = info.ThumbnailMaxUrl;
                        canal.Title = info.Title;
                        canal.Status = EStatusCanal.Ativo;
                    }
                    else
                    {
                        canal.Status = EStatusCanal.CanalNaoExiste;
                    }

                    await _canalRepository.Update(canal);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Reason[quotaExceeded]"))
                    {
                        await _canalRepository.SetSituacao(canal, EStatusCanal.QuotaExceeded);
                    }
                    else
                    {
                        await _canalRepository.SetSituacao(canal, EStatusCanal.Erro, e);
                        throw;
                    }
                }
            }
        }

        private async Task ValidarPlaylists(IEnumerable<Canal> playlists)
        {
            foreach (var playlist in playlists)
            {
                try
                {
                    var (info, cost) = await _youTubeServices.GetPlaylistInfo(playlist.Usuario.ApiKey, playlist.YoutubeCanalId);
                    if (info != null)
                    {
                        playlist.CustomUrl = info.CustomUrl;
                        playlist.Description = info.Description;
                        playlist.PublishedAt = info.PublishedAt;
                        playlist.ThumbnailMinUrl = info.ThumbnailMinUrl;
                        playlist.ThumbnailMediumUrl = info.ThumbnailMediumUrl;
                        playlist.ThumbnailMaxUrl = info.ThumbnailMaxUrl;
                        playlist.Title = info.Title;
                        playlist.Status = EStatusCanal.Ativo;
                    }
                    else
                    {
                        playlist.Status = EStatusCanal.CanalNaoExiste;
                    }

                    await _canalRepository.Update(playlist);
                }
                catch (Exception e)
                {
                    await _canalRepository.SetSituacao(playlist, EStatusCanal.Erro, e);
                    throw;
                }
            }

        }
    }
}
