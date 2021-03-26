using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;
using myTube.Data.Repositories;
using myTube.Domain.Enums;
using myTube.Services.XML;
using myTube.Services.Youtube.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Services.Youtube
{
    public class YoutubeServices
    {
        private readonly ILogger<YoutubeServices> _logger;
        private readonly VideoRepository _videoRepository;
        private readonly LogYoutubeRepository _logYoutubeRepository;

        public YoutubeServices(ILogger<YoutubeServices> logger,
            VideoRepository videoRepository,
            LogYoutubeRepository logYoutubeRepository)
        {
            _logger = logger;
            _videoRepository = videoRepository;
            _logYoutubeRepository = logYoutubeRepository;
        }

        public async Task<(List<YoutubeMovie>, int)> GetVideosByChannelId(string apiKey, Guid usuarioId, string channelId, ESource source, DateTime publishedAfter)
        {
            var (videos, cost) = await GetVideosByChannelIdUsingFeed(apiKey, usuarioId, channelId, source, publishedAfter);
            //var (videos, cost) = await GetVideosByChannelIdUsingApi(apiKey, channelId, publishedAfter);

            if (cost > 0)
            {
                await _logYoutubeRepository.Add("GetVideosByChannelId", cost);
            }

            return (videos, cost);

        }

        private async Task<(List<YoutubeMovie>, int)> GetVideosByChannelIdUsingFeed(string apiKey, Guid usuarioId, string channelId, ESource source, DateTime publishedAfter)
        {
            var result = new List<YoutubeMovie>();

            var cost = 0;
            var listIds = new List<string>();
            var feedList = await GetVideosByFeed(channelId, source);
            foreach (var feed in feedList)
            {

                if (feed.PublishedAt >= publishedAfter)
                {
                    var videoDB = await _videoRepository.GetByYoutubeId(feed.VideoId, usuarioId);
                    if (videoDB == null) // || DateTime.Now >= videoDB.ScheduledStartTime?.AddHours(1) && videoDB.DurationSecs == 0)
                    {
                        result.Add(new YoutubeMovie()
                        {
                            Id = feed.VideoId,
                            ChannelId = feed.ChannelId,
                            PublishedAt = feed.PublishedAt
                        });
                        listIds.Add(feed.VideoId);
                    }
                }

                if (listIds.Count >= 40)
                {
                    cost += await GetExtraVideosInformation(apiKey, string.Join(",", listIds), result);
                    listIds.Clear();
                }
            }

            if (listIds.Count > 0)
            {
                cost += await GetExtraVideosInformation(apiKey, string.Join(",", listIds), result);
            }

            return (result, cost);
        }

        private async Task<List<Entry>> GetVideosByFeed(string channelId, ESource source)
        {
            return await Task.Run(() =>
            {
                var items = new List<Entry>();

                var url = "";
                if (source == ESource.Canal)
                    url = "https://www.youtube.com/feeds/videos.xml?channel_id=" + channelId;
                else if (source == ESource.Playlist)
                    url = "https://www.youtube.com/feeds/videos.xml?playlist_id=" + channelId;

                //var url = @"\\192.168.1.101\downloads\videos.xml";

                var tags = new ReadTagsXML(url, new List<Type>
                        {
                            typeof(Entry)
                        });

                (object obj, string xml) tagRead;
                while ((tagRead = tags.ReadNextTag()).obj != null)
                {
                    items.Add((Entry)tagRead.obj);
                }

                return items;
            });
        }

        private async Task<(List<YoutubeMovie>, int)> GetVideosByChannelIdUsingApi(string apiKey, string channelId, DateTime publishedAfter)
        {
            var result = new List<YoutubeMovie>();

            var listIds = new List<string>();
            var (videos, cost) = await _GetVideosByChannelId(apiKey, channelId, publishedAfter);
            foreach (var video in videos)
            {
                result.Add(new YoutubeMovie()
                {
                    Id = video.Id.VideoId,
                    ChannelId = video.Snippet.ChannelId,
                    Title = video.Snippet.Title,
                    Summary = video.Snippet.Description,
                    ETag = video.ETag,
                    PublishedAt = video.Snippet.PublishedAt,
                    ThumbnailMinUrl = video.Snippet.Thumbnails.Default__.Url.Replace("_live", ""),
                    ThumbnailMediumUrl = video.Snippet.Thumbnails.Medium.Url.Replace("_live", ""),
                    ThumbnailMaxUrl = video.Snippet.Thumbnails.High.Url.Replace("_live", ""),
                });
                listIds.Add(video.Id.VideoId);

                if (listIds.Count >= 40)
                {
                    cost += await GetExtraVideosInformation(apiKey, string.Join(",", listIds), result);
                    listIds.Clear();
                }
            }

            if (listIds.Count > 0)
            {
                cost += await GetExtraVideosInformation(apiKey, string.Join(",", listIds), result);
            }

            return (result, cost);
        }

        public async Task<(YoutubeChannel, int)> GetChannelInfo(string apiKey, string youtubeCanalId)
        {
            return await Task.Run(async () =>
            {
                YoutubeChannel ret = null;
                var cost = 0;

                var _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey,
                    ApplicationName = "Videopedia"//this.GetType().ToString()
                });

                var searchListRequest = _youtubeService.Channels.List("snippet");
                searchListRequest.Id = youtubeCanalId;
                searchListRequest.MaxResults = 50;
                //searchListRequest.MySubscribers = true;
                searchListRequest.PageToken = " ";

                try
                {
                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.Execute();
                    cost += 1;

                    // Process  the video responses 
                    //res.AddRange(searchListResponse.Items);

                    if (searchListResponse.Items != null && searchListResponse.Items.Count > 0)
                    {
                        ret = new YoutubeChannel()
                        {
                            CustomUrl = searchListResponse.Items[0].Snippet.CustomUrl,
                            Description = searchListResponse.Items[0].Snippet.Description,
                            PublishedAt = (DateTime)searchListResponse.Items[0].Snippet.PublishedAt,
                            ThumbnailMinUrl = searchListResponse.Items[0].Snippet.Thumbnails.Default__.Url.Replace("_live", ""),
                            ThumbnailMediumUrl = searchListResponse.Items[0].Snippet.Thumbnails.Medium.Url.Replace("_live", ""),
                            ThumbnailMaxUrl = searchListResponse.Items[0].Snippet.Thumbnails.High.Url.Replace("_live", ""),
                            Title = searchListResponse.Items[0].Snippet.Title,
                            ETag = searchListResponse.Items[0].Snippet.ETag,
                            Id = searchListResponse.Items[0].Id
                        };
                    }
                }
                catch
                {

                }

                await _logYoutubeRepository.Add("GetChannelInfo", cost);

                return (ret, cost);
            });
        }

        public async Task<(YoutubeChannel, int)> GetPlaylistInfo(string apiKey, string youtubePlaylistId)
        {
            return await Task.Run(async () =>
            {
                YoutubeChannel ret = null;
                var cost = 0;

                var _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey,
                    ApplicationName = "Videopedia"//this.GetType().ToString()
                });

                var searchListRequest = _youtubeService.Playlists.List("snippet");
                searchListRequest.Id = youtubePlaylistId;
                searchListRequest.MaxResults = 50;
                //searchListRequest.MySubscribers = true;
                searchListRequest.PageToken = " ";

                try
                {
                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.Execute();
                    cost += 1;

                    if (searchListResponse.Items != null && searchListResponse.Items.Count > 0)
                    {
                        ret = new YoutubeChannel()
                        {
                            Description = searchListResponse.Items[0].Snippet.Description,
                            PublishedAt = (DateTime)searchListResponse.Items[0].Snippet.PublishedAt,
                            ThumbnailMinUrl = searchListResponse.Items[0].Snippet.Thumbnails.Default__.Url.Replace("_live", ""),
                            ThumbnailMediumUrl = searchListResponse.Items[0].Snippet.Thumbnails.Medium.Url.Replace("_live", ""),
                            ThumbnailMaxUrl = searchListResponse.Items[0].Snippet.Thumbnails.High.Url.Replace("_live", ""),
                            Title = searchListResponse.Items[0].Snippet.Title,
                            ETag = searchListResponse.Items[0].Snippet.ETag,
                            Id = searchListResponse.Items[0].Id
                        };
                    }
                }
                catch
                {

                }

                await _logYoutubeRepository.Add("GetChannelInfo", cost);

                return (ret, cost);
            });
        }

        private async Task<int> GetExtraVideosInformation(string apiKey, string videoIds, List<YoutubeMovie> listMovies)
        {
            try
            {
                var (list, cost) = await GetInfoVideoAsync(apiKey, videoIds);
                foreach (var video in list)
                {
                    var videoInList = LocateVideoById(video.Id, listMovies);
                    if (videoInList != null)
                    {
                        videoInList.Description = video.Snippet.Description;
                        videoInList.DurationSecs = YoutubeTimeToSecs(video.ContentDetails.Duration);
                        videoInList.Title = video.Snippet.Title;
                        videoInList.ETag = video.Snippet.ETag;
                        videoInList.Summary = video.Snippet.Description;

                        videoInList.ThumbnailMinUrl = video.Snippet.Thumbnails.Default__.Url.Replace("_live", "");
                        videoInList.ThumbnailMediumUrl = video.Snippet.Thumbnails.Medium.Url.Replace("_live", "");
                        videoInList.ThumbnailMaxUrl = video.Snippet.Thumbnails.High.Url.Replace("_live", "");

                        if (video.LiveStreamingDetails != null)
                        {
                            videoInList.ScheduledStartTime = video.LiveStreamingDetails.ScheduledStartTime;
                        }
                    }
                }

                return cost;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            return 0;
        }

        private static double YoutubeTimeToSecs(string youtubeTime)
        {
            var x = System.Xml.XmlConvert.ToTimeSpan(youtubeTime);

            return x.TotalSeconds;
        }

        private static YoutubeMovie LocateVideoById(string id, List<YoutubeMovie> listMovies)
        {
            foreach (var found in listMovies)
            {
                if (found.Id == id)
                {
                    return found;
                }
            }

            return null;
        }

        private static async Task<(List<Video>, int)> GetInfoVideoAsync(string apiKey, string idVideos)
        {

            return await Task.Run(() =>
            {
                List<Video> res = new();
                var cost = 0;

                var _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey,
                    ApplicationName = "Videopedia"//this.GetType().ToString()
                });

                string nextpagetoken = " ";

                while (nextpagetoken != null)
                {
                    var searchListRequest = _youtubeService.Videos.List("snippet,contentDetails,LiveStreamingDetails");
                    searchListRequest.Id = idVideos;
                    searchListRequest.MaxResults = 1;
                    //searchListRequest.MySubscribers = true;
                    //searchListRequest.PageToken = nextpagetoken;

                    try
                    {
                        // Call the search.list method to retrieve results matching the specified query term.
                        var searchListResponse = searchListRequest.Execute();
                        cost += 1;

                        // Process  the video responses 
                        res.AddRange(searchListResponse.Items);

                        nextpagetoken = searchListResponse.NextPageToken;
                    }
                    catch
                    {
                        throw;
                    }
                }

                return (res, cost);

            });
        }

        private static async Task<(List<SearchResult>, int)> _GetVideosByChannelId(string apiKey, string channelId, DateTime publishedAfter)
        {

            return await Task.Run(() =>
            {
                List<SearchResult> res = new();
                var cost = 0;

                var _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey,
                    ApplicationName = "Videopedia"//this.GetType().ToString()
                });

                string nextpagetoken = " ";

                while (nextpagetoken != null)
                {
                    var searchListRequest = _youtubeService.Search.List("snippet");
                    searchListRequest.MaxResults = 50;
                    searchListRequest.ChannelId = channelId;
                    searchListRequest.Type = "video";
                    searchListRequest.PageToken = nextpagetoken;
                    searchListRequest.PublishedAfter = publishedAfter;

                    // Call the search.list method to retrieve results matching the specified query term.
                    try
                    {
                        var searchListResponse = searchListRequest.Execute();
                        cost += 100;

                        // Process the video responses 
                        res.AddRange(searchListResponse.Items);

                        nextpagetoken = searchListResponse.NextPageToken;
                    }
                    catch
                    {
                        throw;
                    }
                }

                return (res, cost);

            });
        }

    }
}
