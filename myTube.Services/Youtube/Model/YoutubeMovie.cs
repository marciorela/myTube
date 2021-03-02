using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Services.Youtube.Model
{
    public class YoutubeMovie
    {
        public string Id { get; set; }
        public string ChannelId { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string ETag { get; set; }
        public double DurationSecs { get; set; }

        public string ThumbnailMaxUrl { get; set; }
        public string ThumbnailMediumUrl { get; set; }
        public string ThumbnailMinUrl { get; set; }
    }
}
