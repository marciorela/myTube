using myTube.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Entities
{
    public class LogYoutube : Entity
    {
        [Required]
        [StringLength(40)]
        public string Service { get; set; }

        [Required]
        public int Cost { get; set; }

        public string Error { get; set; }

    }
}
