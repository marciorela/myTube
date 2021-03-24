using Microsoft.AspNetCore.Mvc;
using myTube.Data.Repositories;
using myTube.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Models.Components
{
    public class HeaderInfo : ViewComponent
    {
        private readonly VideoRepository _videoRepository;
        private readonly UsuarioService _usuarioService;

        public HeaderInfo(VideoRepository videoRepository, UsuarioService usuarioService)
        {
            _videoRepository = videoRepository;
            _usuarioService = usuarioService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cont = (await _videoRepository.GetListIndex(_usuarioService.Id)).Count();
            return View(cont);
        }
    }

}
