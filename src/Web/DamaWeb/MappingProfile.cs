using ApplicationCore.Entities;
using AutoMapper;
using DamaWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DamaWeb
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ShopConfigDetail, MainBannerViewModel>();
            CreateMap<MainBannerViewModel, ShopConfigDetail>();
        }
    }
}
