﻿using System.Threading.Tasks;

namespace DamaNoJornal.Core.Services.Settings
{
    public interface ISettingsService
    {
        string AuthAccessToken { get; set; }
        string AuthIdToken { get; set; }
        bool UseMocks { get; set; }
        string UrlBase { get; set; }
        bool UseFakeLocation { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
        bool AllowGpsLocation { get; set; }
        string JueUserId { get; set; }
        string SueUserId { get; set; }
        string MotherUserId { get; set; }
        string PlaceId { get; set; }

        bool GetValueOrDefault(string key, bool defaultValue);
        string GetValueOrDefault(string key, string defaultValue);
        Task AddOrUpdateValue(string key, bool value);
        Task AddOrUpdateValue(string key, string value);
    }
}