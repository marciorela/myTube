using myTube.Domain.Base;
using myTube.Domain.Enums;
using myTube.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Entities
{
    public class Usuario : Entity
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(150)]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string ApiKey { get; set; }

        [Required]
        public EStatusUsuario Status { get; set; } = EStatusUsuario.Ativo;
    }
}
