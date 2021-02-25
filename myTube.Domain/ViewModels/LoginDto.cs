using myTube.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.ViewModels
{
    public class LoginDto
    {

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Senha deve ser informada.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
