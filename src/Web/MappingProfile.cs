using ApplicationCore.Entities;
using AutoMapper;
using Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
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
