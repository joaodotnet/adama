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
using Microsoft.Extensions.Caching.Memory;

namespace DamaWeb.Services
{
    public class CacheShopService : IShopService
    {
        private readonly IMemoryCache _cache;
        private readonly ShopService _shopService;
        private static readonly string _bannersKey = "DAMA_CONFIG";
        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromMinutes(60);

        public CacheShopService(IMemoryCache cache, ShopService shopService)
        {
            _cache = cache;
            _shopService = shopService;
        }

        public async Task<DamaHomePageConfigViewModel> GetDamaHomePageConfig()
        {
            return await _cache.GetOrCreateAsync(_bannersKey, async entry =>
            {
                entry.SlidingExpiration = _defaultCacheDuration;
                return await _shopService.GetDamaHomePageConfig();
            });
        }        
    }
}
