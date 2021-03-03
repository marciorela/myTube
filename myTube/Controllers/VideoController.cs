﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myTube.Data.Repositories;
using myTube.Domain.Enums;
using myTube.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Controllers
{
    
    [Authorize]
    public class VideoController : Controller
    {
        private readonly VideoRepository _videoRepository;
        private readonly UsuarioService _usuarioService;

        public VideoController(VideoRepository videoRepository, UsuarioService usuarioService)
        {
            _videoRepository = videoRepository;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var videos = await _videoRepository.GetByStatus(_usuarioService.Id, EStatusVideo.NaoAssistido);

            return View(videos);
        }
    }
}
