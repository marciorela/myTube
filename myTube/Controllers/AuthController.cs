using Microsoft.AspNetCore.Mvc;
using MR.String;
using myTube.Data.Repositories;
using myTube.Domain.ViewModels;
using myTube.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly UsuarioService _usuarioService;

        public AuthController(UsuarioRepository usuarioRepository, UsuarioService usuarioService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return PartialView("_SignIn", new LoginDto());
        }

        [HttpPost]
        public async Task <IActionResult> SignIn(string returnUrl, LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var usuario = await _usuarioRepository.GetByEmailAndPassoword(login.Email, login.Password.Encrypt());
            if (usuario == null)
            {
                TempData["msg-error"] = "Usuário e/ou senha inválidos";
                return View(login);
            }

            await _usuarioService.SignIn(usuario);

            return Redirect(returnUrl ?? "/");

        }
    }
}
