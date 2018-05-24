﻿using System;
using System.Threading.Tasks;
using DamaNoJornal.Core.Services.RequestProvider;

namespace DamaNoJornal.Core.Services.Location
{
    public class LocationService : ILocationService
    {
        private readonly IRequestProvider _requestProvider;

        public LocationService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task UpdateUserLocation(DamaNoJornal.Core.Models.Location.Location newLocReq, string token)
        {
            UriBuilder builder = new UriBuilder(GlobalSetting.Instance.BaseEndpoint);
            builder.Path = "/mobilemarketingapigw/api/v1/l/locations";
            string uri = builder.ToString();
            await _requestProvider.PostAsync(uri, newLocReq, token);
        }
    }
}