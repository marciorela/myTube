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
using MR.String;
using myTube.Domain.Entities;

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
        public IActionResult Total()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var videos = await _videoRepository.GetListIndex(_usuarioService.Id);
            foreach (var video in videos)
            {
                video.WatchedSecs = video.WatchedSecs > 30 ? Math.Max(video.WatchedSecs - 15, 0) : 0;
            }

            return View(videos);
        }

        [HttpGet]
        public async Task<IActionResult> PlayVideo(Guid id)
        {
            var video = await _videoRepository.GetById(id);

            return PartialView("_PlayVideo", video);
        }

        [HttpGet]
        public async Task<IActionResult> Progress(Guid id, double progress)
        {
            await _videoRepository.SetProgress(id, progress);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Assistido(Guid id)
        {
            await _videoRepository.ChangeStatus(id, EStatusVideo.Assistido);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Ignorado(Guid id)
        {
            await _videoRepository.ChangeStatus(id, EStatusVideo.Ignorado);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Favorito(Guid id)
        {
            await _videoRepository.ChangeStatus(id, EStatusVideo.Favorito);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AssistirDepois(Guid id)
        {
            await _videoRepository.ChangeStatus(id, EStatusVideo.AssistirDepois);

            return Ok();
        }
    }
}
