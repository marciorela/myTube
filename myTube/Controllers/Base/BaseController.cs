using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myTube.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Controllers.Base
{
    public class BaseController : Controller
    {
        private readonly VideoRepository _videoRepository;
        private readonly IHttpContextAccessor _accessor;

        public BaseController(VideoRepository videoRepository, IHttpContextAccessor accessor)
        {
            _videoRepository = videoRepository;
            _accessor = accessor;

            //HttpContext.Current Session["Info"] = "Qualquer info";
        }
    }
}
