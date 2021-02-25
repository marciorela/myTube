using AutoMapper;
using myTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Automapper
{
    public class CanalProfile : Profile
    {
        public CanalProfile()
        {
            CreateMap<Canal, Canal>();
        }
    }
}
