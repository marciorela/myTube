using myTube.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Entities
{
    public class Filme : Entity
    {
        [Required]
        [StringLength(50)]
        public string YoutubeFilmeId { get; set; }

        [Required]
        [StringLength(50)]
        public string ETag { get; set; }

        [StringLength(200)]
        public string Title { get; set; }
        
        public string Summary { get; set; }

        public string Description { get; set; }
        
        public double DurationSecs { get; set; }
        
        public DateTime? PublishedAt { get; set; }
        
        [StringLength(150)]
        public string ThumbnailMinUrl { get; set; }

        [StringLength(150)]
        public string ThumbnailMediumUrl { get; set; }

        [StringLength(150)]
        public string ThumbnailMaxUrl { get; set; }

        public DateTime? AssistidoEm { get; set; }

        [Required]
        public bool Assistido { get; set; } = false;

        
        // FK
        public Guid CanalId { get; set; }
        public virtual Canal Canal { get; set; }
    }
}
