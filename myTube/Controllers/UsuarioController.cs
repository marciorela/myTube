using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(IMapper mapper, UsuarioRepository usuarioRepository)
        {
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UsuarioDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(string returnUrl, UsuarioDto usuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var novoUsuario = await _usuarioRepository.GetByEmailAsync(usuarioDto.Email);
            if (novoUsuario == null)
            {
                novoUsuario = _mapper.Map<Usuario>(usuarioDto);

                novoUsuario.Password = usuarioDto.Password.Encrypt();

                //novoUsuario = new Usuario()
                //{
                //    ApiKey = usuario.ApiKey,
                //    Email = usuario.Email,
                //    Nome = usuario.Nome,
                //    Password = usuario.Password.Encrypt(),
                //    Status = EStatusUsuario.Ativo
                //};

                await _usuarioRepository.AddAsync(novoUsuario);
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                TempData["msg-error"] = "E-mail já cadastrado.";
                return View(usuarioDto);
            }

        }
    }
}
