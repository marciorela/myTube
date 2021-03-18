using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using myTube.Data.Repositories;
using myTube.Domain.Entities;
using myTube.Services;
using myTube.Services.Youtube;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace myTube.WS
{
    public class WSCheck : BackgroundService
    {
        private readonly ILogger<WSCheck> _logger;
        private readonly CheckVideoService _checkVideoService;
        private readonly CheckChannelService _checkChannelService;

        public WSCheck(
            ILogger<WSCheck> logger,
            CheckVideoService checkVideoService,
            CheckChannelService checkChannelService
            )
        {
            _logger = logger;
            _checkVideoService = checkVideoService;
            _checkChannelService = checkChannelService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // SOMENTE ENTRE 07:00 E 23:59
                    if (DateTime.Now.Hour >= 7)
                    {

                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                        try
                        {
                            await _checkChannelService.ValidarCanaisPendentes();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, e.Message);
                        }

                        try
                        {
                            await _checkVideoService.GetMovies();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, e.Message);
                        }
                    }
                } 
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }

                _logger.LogInformation("Aguardando...");
                await Task.Delay(60000, stoppingToken);
            }
        }

    }
}
