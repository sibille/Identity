﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ForcedLogInApp.Core.Helpers;
using ForcedLogInApp.Core.Models;

namespace ForcedLogInApp.Core.Services
{
    public class MicrosoftGraphService
    {
        // TODO WTS: Checkout Microsoft Graph Explorer
        // https://developer.microsoft.com/graph/graph-explorer
        //
        // Checkout Get-User Service Documentation
        // https://docs.microsoft.com/graph/api/user-get?view=graph-rest-1.0
        //
        private const string _graphAPIEndpoint = "https://graph.microsoft.com/v1.0/";
        private const string _apiServiceMe = "me/";
        private const string _apiServiceMePhoto = "me/photo/$value";

        public MicrosoftGraphService()
        {
        }

        public async Task<User> GetUserInfoAsync(string accessToken)
        {
            User user = null;
            var httpContent = await GetDataAsync($"{_graphAPIEndpoint}{_apiServiceMe}", accessToken);
            if (httpContent != null)
            {
                var userData = await httpContent.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(userData))
                {
                    user = await Json.ToObjectAsync<User>(userData);
                }
            }            

            return user;
        }

        public async Task<string> GetUserPhoto(string accessToken)
        {
            var httpContent = await GetDataAsync($"{_graphAPIEndpoint}{_apiServiceMePhoto}", accessToken);

            if (httpContent == null)
            {
                return string.Empty;
            }

            var stream = await httpContent.ReadAsStreamAsync();
            return stream.ToBase64String();
        }

        private async Task<HttpContent> GetDataAsync(string url, string accessToken)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await httpClient.SendAsync(request);                    
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content;
                    }
                }
            }
            catch (Exception)
            {
                // TODO WTS: This call can fail please handle exceptions as appropriate to your scenario
            }

            return null;
        }
    }
}
