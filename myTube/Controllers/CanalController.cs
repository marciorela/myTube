using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MR.String;
using myTube.Data.Repositories;
using myTube.Domain.Entities;
using myTube.Domain.Enums;
using myTube.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTube.Controllers
{

    [Authorize]
    public class CanalController : Controller
    {
        private readonly CanalRepository _canalRepository;
        private readonly UsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public CanalController(CanalRepository canalRepository, UsuarioService usuarioService, IMapper mapper)
        {
            _canalRepository = canalRepository;
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        // GET: CanalController
        public async Task<IActionResult> Index()
        {
            var all = await _canalRepository.GetAll(_usuarioService.Id);

            return View(all);
        }

        // GET: CanalController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CanalController/Create
        public ActionResult Create()
        {
            var values = Enum.GetValues(typeof(ESource));
            var items = new List<SelectListItem>(values.Length);
            
            foreach (var value in values)
            {
                items.Add(new SelectListItem
                {
                    //Text = ((ESource)value).GetDescription(),
                    Text = value.ToString(),
                    Value = ((int)value).ToString()
                });
            }

            ViewBag.SourceItems = items;
            var canal = new Canal
            {
                PrimeiraBusca = DateTime.Now.AddDays(-30),
                Source = ESource.Canal
            };

            return View(canal);
        }

        // POST: CanalController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Canal canal)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkCanal = await _canalRepository.GetByYoutubeId(_usuarioService.Id, canal.YoutubeCanalId);
                    if (checkCanal == null)
                    {
                        canal.UsuarioId = _usuarioService.Id;

                        await _canalRepository.Add(canal);
                    }
                    else
                    {
                        TempData["msg-error"] = "Canal já cadastrado.";
                        return View(canal);
                    }

                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(canal);
            }
        }

        // GET: CanalController/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View();
        }

        // POST: CanalController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CanalController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CanalController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
