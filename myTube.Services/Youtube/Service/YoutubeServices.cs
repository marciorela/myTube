using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;
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

        public YoutubeServices(ILogger<YoutubeServices> logger)
        {
            _logger = logger;
        }

        public async Task<List<YoutubeMovie>> GetVideosByChannelId(string apiKey, string channelId, DateTime publishedAfter)
        {
            var result = new List<YoutubeMovie>();

            var listIds = new List<string>();
            var videos = await _GetVideosByChannelId(apiKey, channelId, publishedAfter);
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
                    ThumbnailMinUrl = video.Snippet.Thumbnails.Default__.Url,
                    ThumbnailMediumUrl = video.Snippet.Thumbnails.Medium.Url,
                    ThumbnailMaxUrl = video.Snippet.Thumbnails.High.Url
                });
                listIds.Add(video.Id.VideoId);

                if (listIds.Count >= 40)
                {
                    await GetExtraVideosInformation(apiKey, string.Join(",", listIds), result);
                    listIds.Clear();
                }
            }

            if (listIds.Count > 0)
            {
                await GetExtraVideosInformation(apiKey, string.Join(",", listIds), result);
            }

            return result;
        }

        public async Task<YoutubeChannel> GetChannelInfo(string apiKey, string youtubeCanalId)
        {
            return await Task.Run(() =>
            {
                List<Channel> res = new List<Channel>();

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

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = searchListRequest.Execute();

                // Process  the video responses 
                //res.AddRange(searchListResponse.Items);

                if (searchListResponse.Items != null && searchListResponse.Items.Count > 0)
                {
                    return new YoutubeChannel()
                    {
                        CustomUrl = searchListResponse.Items[0].Snippet.CustomUrl,
                        Description = searchListResponse.Items[0].Snippet.Description,
                        PublishedAt = (DateTime)searchListResponse.Items[0].Snippet.PublishedAt,
                        ThumbnailMinUrl = searchListResponse.Items[0].Snippet.Thumbnails.Default__.Url.Replace("_live",""),
                        ThumbnailMediumUrl = searchListResponse.Items[0].Snippet.Thumbnails.Medium.Url.Replace("_live",""),
                        ThumbnailMaxUrl = searchListResponse.Items[0].Snippet.Thumbnails.High.Url.Replace("_live",""),
                        Title = searchListResponse.Items[0].Snippet.Title,
                        ETag = searchListResponse.Items[0].Snippet.ETag,
                        Id = searchListResponse.Items[0].Id
                    };
                }
                else
                {
                    return null;
                }

            });
        }

        private async Task GetExtraVideosInformation(string apiKey, string videoIds, List<YoutubeMovie> listMovies)
        {
            try
            {
                var list = await GetInfoVideoAsync(apiKey, videoIds);
                foreach (var video in list)
                {
                    var videoInList = LocateVideoById(video.Id, listMovies);
                    if (videoInList != null)
                    {
                        videoInList.Description = video.Snippet.Description;
                        videoInList.DurationSecs = YoutubeTimeToSecs(video.ContentDetails.Duration);

                        if (video.LiveStreamingDetails != null)
                        {
                            videoInList.ScheduledStartTime = video.LiveStreamingDetails.ScheduledStartTime;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
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

        private static async Task<List<Video>> GetInfoVideoAsync(string apiKey, string idVideos)
        {

            return await Task.Run(() =>
            {
                List<Video> res = new List<Video>();

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

                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.Execute();

                    // Process  the video responses 
                    res.AddRange(searchListResponse.Items);

                    nextpagetoken = searchListResponse.NextPageToken;

                }

                return res;

            });
        }

        private static async Task<List<SearchResult>> _GetVideosByChannelId(string apiKey, string channelId, DateTime publishedAfter)
        {

            return await Task.Run(() =>
            {
                List<SearchResult> res = new List<SearchResult>();

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
                    searchListRequest.PageToken = nextpagetoken;
                    searchListRequest.PublishedAfter = publishedAfter;

                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.Execute();

                    // Process the video responses 
                    res.AddRange(searchListResponse.Items);

                    nextpagetoken = searchListResponse.NextPageToken;
                }

                return res;

            });
        }

    }
}
