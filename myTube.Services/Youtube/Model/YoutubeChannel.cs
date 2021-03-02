using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Services.Youtube.Model
{
    public class YoutubeChannel
    {

        public string ETag { get; set; }

        public string Id { get; set; }

        public string CustomUrl { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public DateTime PublishedAt { get; set; }

        public string ThumbnailMaxUrl { get; set; }
        public string ThumbnailMediumUrl { get; set; }
        public string ThumbnailMinUrl { get; set; }
    }
}
