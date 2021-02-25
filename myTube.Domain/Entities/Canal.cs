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

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        public DateTime? UltimaBusca { get; set; }

        [Required]
        public EStatusCanal Status { get; set; } = EStatusCanal.Ativo;


        // FK
        [Required]
        public Guid UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
