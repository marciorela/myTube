using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using myTube.Data.Repositories;
using myTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace myTube.WS
{
    public class WSCheckNewVideos : BackgroundService
    {
        private readonly ILogger<WSCheckNewVideos> _logger;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly CanalRepository _canalRepository;
        private readonly FilmeRepository _filmeRepository;

        public WSCheckNewVideos(
            ILogger<WSCheckNewVideos> logger, 
            UsuarioRepository usuarioRepository,
            CanalRepository canalRepository,
            FilmeRepository filmeRepository
            )
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
            _canalRepository = canalRepository;
            _filmeRepository = filmeRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    //await GetMovies();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "err");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task GetMovies()
        {
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
                var filmes = await GetMoviesByCanal(canal);
                foreach (var filme in filmes)
                {

                    var filmeDB = await _filmeRepository.GetByYoutubeId(filme.Id.VideoId, usuario.Id);
                    if (filmeDB == null)
                    {
                        await _filmeRepository.Add(new Filme
                        {
                            YoutubeFilmeId = filme.Id.VideoId,
                            CanalId = canal.Id,
                            PublishedAt = filme.Snippet.PublishedAt,
                            Description = filme.Snippet.Description,
                            ThumbnailMaxUrl = filme.Snippet.Thumbnails.High?.Url,
                            ThumbnailMediumUrl = filme.Snippet.Thumbnails.Medium?.Url,
                            ThumbnailMinUrl = filme.Snippet.Thumbnails.Default__?.Url,
                            Title = filme.Snippet.Title,
                            ETag = filme.Snippet.ETag ?? ""
                        });
                    }

                }
            }
        }

        public async Task<List<SearchResult>> GetMoviesByCanal(Canal canal)
        {

            return await Task.Run(() =>
            {
                List<SearchResult> res = new List<SearchResult>();

                var _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = canal.Usuario.ApiKey,
                    ApplicationName = "Videopedia"//this.GetType().ToString()
                });

                string nextpagetoken = " ";

                while (nextpagetoken != null)
                {
                    var searchListRequest = _youtubeService.Search.List("snippet");
                    //searchListRequest.MaxResults = 50;
                    searchListRequest.ChannelId = canal.YoutubeCanalId;
                    searchListRequest.PageToken = nextpagetoken;

                    // SEMPRE BUSCAR A PARTIR DAS 0:00 DO DIA ANTERIOR À ÚLTIMA BUSCA
                    if (canal.UltimaBusca == null)
                    {
                        searchListRequest.PublishedAfter = DateTime.Today.AddDays(-1);
                    }
                    else
                    {
                        searchListRequest.PublishedAfter = ((DateTime)canal.UltimaBusca).Date.AddDays(-1);
                    }

                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.Execute();

                    // Process  the video responses 
                    res.AddRange(searchListResponse.Items);

                    nextpagetoken = searchListResponse.NextPageToken;
                }

                return res;

            });
        }
    }
}
