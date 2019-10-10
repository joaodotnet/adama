using ApplicationCore.Entities;
using AutoMapper;
using DamaWeb.Interfaces;
using DamaWeb.ViewModels;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Infrastructure.Identity;
using ApplicationCore.Interfaces;

namespace DamaWeb.Services
{
    public class ShopService : IShopService
    {
        private readonly DamaContext _db; //TODO Replace by repository
        private readonly IMapper _mapper;
        public ShopService(DamaContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<DamaHomePageConfigViewModel> GetDamaHomePageConfig()
        {
            var configs = await _db.ShopConfigs
                .Include(x => x.Details)
                .Where(x => x.IsActive)
                .ToListAsync();

            return new DamaHomePageConfigViewModel
            {
                MetaDescription = configs.SingleOrDefault(x => x.Type == ShopConfigType.SEO && x.Name == "Meta Description")?.Value,
                Title = configs.SingleOrDefault(x => x.Type == ShopConfigType.SEO && x.Name == "Title")?.Value,
                Banners = _mapper.Map<List<MainBannerViewModel>>(configs.SingleOrDefault(x => x.Type == ShopConfigType.NEWS_BANNER)?
                .Details
                .Where(d => d.IsActive)
                .ToList())
            };
        }         
    }
}
