using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using myTube.Data.Repositories;
using myTube.Domain.Enums;
using myTube.Services.Youtube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myTube.WS
{
    public class WSValidateChannel : BackgroundService
    {
        private ILogger<WSValidateChannel> _logger;
        private CanalRepository _canalRepository;
        private YoutubeServices _youTubeServices;

        public WSValidateChannel(
            ILogger<WSValidateChannel> logger,
            CanalRepository canalRepository,
            YoutubeServices youTubeServices)
        {
            _logger = logger;
            _canalRepository = canalRepository;
            _youTubeServices = youTubeServices;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    await ValidarCanaisPendentes();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ValidarCanaisPendentes()
        {
            var canais = await _canalRepository.GetByStatus(EStatusCanal.Validar);
            foreach (var canal in canais)
            {
                var info = await _youTubeServices.GetChannelInfo(canal.Usuario.ApiKey, canal.YoutubeCanalId);

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

                    await _canalRepository.Update(canal);
                }
            }
        }
    }
}
