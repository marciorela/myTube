using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myTube.Controllers.Base;
using myTube.Data;
using myTube.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Controllers
{
    public class AdminController : BaseController
    {
        private readonly AppDbContext _ctx;

        public AdminController(AppDbContext ctx, VideoRepository videoRepository, IHttpContextAccessor accessor) : base(videoRepository, accessor)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Migrate()
        {
            try
            {
                var list = await _ctx.Database.GetPendingMigrationsAsync();
                if (list.Any())
                {
                    await _ctx.Database.MigrateAsync();
                }

                return RedirectToAction("Index", "Video");
            }
            catch (Exception e)
            {
                return View("ViewError", e);
            }


            //return View(nameof(VideoController));
        }
    }
}
