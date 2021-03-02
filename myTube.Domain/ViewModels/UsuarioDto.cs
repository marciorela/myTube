using myTube.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.ViewModels
{
    public class UsuarioDto
    {
        [Required(ErrorMessage = "E-mail deve ser informado")]
        [Display(Name = "E-mail")]
        //[DataType(DataType.EmailAddress, ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nome deve ser informado")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "ApiKey deve ser informada")]
        public string ApiKey { get; set; }

        [Required(ErrorMessage = "Senha deve ser informada")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Senhas não conferem")]
        [Display(Name = "Redigite a senha")]
        public string RetypePassword { get; set; }
    }
}
