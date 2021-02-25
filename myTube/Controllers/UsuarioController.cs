using Microsoft.AspNetCore.Mvc;
using MR.String;
using myTube.Data.Repositories;
using myTube.Domain.Entities;
using myTube.Domain.Enums;
using myTube.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UsuarioDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(string returnUrl, UsuarioDto usuario)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var novoUsuario = await _usuarioRepository.GetByEmailAsync(usuario.Email);
            if (novoUsuario == null)
            {
                novoUsuario = new Usuario()
                {
                    ApiKey = usuario.ApiKey,
                    Email = usuario.Email,
                    Nome = usuario.Nome,
                    Password = usuario.Password.Encrypt(),
                    Status = EStatusUsuario.Ativo
                };

                await _usuarioRepository.AddAsync(novoUsuario);
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                TempData["msg-error"] = "E-mail já cadastrado.";
                return View(usuario);
            }

        }
    }
}
