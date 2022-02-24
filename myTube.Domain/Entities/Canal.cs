using myTube.Domain.Base;
using myTube.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Entities
{
    public class Canal : Entity
    {
        [Required]
        [StringLength(50)]
        public string YoutubeCanalId { get; set; }

        [Display(Name = "Última Busca")]
        public DateTime? UltimaBusca { get; set; }
        
        [Display(Name = "Último Vídeo")]
        public DateTime? UltimoVideo { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime PrimeiraBusca  { get; set; }

        [Required]
        public ESource Source { get; set; } = ESource.Canal;

        [StringLength(50)]
        public string Categoria { get; set; }

        // READ-ONLY - INFORMAÇÕES CONSULTADAS NA API
        [StringLength(50)]
        public string ETag { get; set; }

        [StringLength(200)]
        public string CustomUrl { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [StringLength(150)]
        public string ThumbnailMinUrl { get; set; }

        [StringLength(150)]
        public string ThumbnailMediumUrl { get; set; }

        [StringLength(150)]
        public string ThumbnailMaxUrl { get; set; }

        public DateTime? PublishedAt { get; set; }

        [Required]
        public EStatusCanal Status { get; set; } = EStatusCanal.Validar;


        // FK
        [Required]
        public Guid UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
