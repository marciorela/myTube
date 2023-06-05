using Microsoft.AspNetCore.Authorization;
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
using myTube.Models.DTO;
using MR.PagedList;

namespace myTube.Controllers
{
    
    [Authorize]
    public class VideoController : Controller
    {
        private readonly VideoRepository _videoRepository;
        private readonly CanalRepository _canalRepository;
        private readonly UsuarioService _usuarioService;

        public VideoController(VideoRepository videoRepository, CanalRepository canalRepository, UsuarioService usuarioService)
        {
            _videoRepository = videoRepository;
            _canalRepository = canalRepository;
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Total()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Index(PesquisaDTO pesquisa, [FromQuery] string page = "1")
        {
            var videos = await _videoRepository.GetListIndex(_usuarioService.Id, pesquisa.CanalId, int.Parse(page), pesquisa.Watch, pesquisa.Categoria);
            foreach (var video in videos.Items)
            {
                if (video.Status == EStatusVideo.NaoAssistido)
                {
                    video.WatchedSecs = video.WatchedSecs > 30 ? Math.Max(video.WatchedSecs - 15, 0) : 0;
                }
                else
                {
                    video.WatchedSecs = 0;
                }
            }
            ViewBag.Categorias = await _canalRepository.CategoriasPorUsuario(_usuarioService.Id);

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
