﻿using BaseLibrary.DTOs;
using ClientLibrary.Services.Contracts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.Helpers
{
    public class CustomHttpHandler
        (GetHttpClient getHttpClient, LocalStorageService localStorageService,IUserAccountService accountService) : DelegatingHandler
    {

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login");
            bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("register");
            bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("refresh-token");

            if (loginUrl || registerUrl || refreshTokenUrl) return await base.SendAsync(request, cancellationToken);
       
            var result = await base.SendAsync(request, cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Get token from localStorage
                var stringToken = await localStorageService.GetToken();
                if (stringToken == null) return result;
                // check if the header containers token
                string token = string.Empty;
                try { token = request.Headers.Authorization!.Parameter!; }
                catch { }
                var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
                if (deserializeToken is null) return result;

                if (string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializeToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }

                // Call for refresh token
                var newJwtToken = await GetFreshToken(deserializeToken.RefreshToken!);
                if (string.IsNullOrEmpty(newJwtToken)) return result;

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newJwtToken);
                return await base.SendAsync(request, cancellationToken);
            }
            return result;
        }

        private async Task<string> GetFreshToken(string refreshToken)
        {
            try
            {
                var result = await accountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    Console.WriteLine("Failed to refresh token: result is null or token is empty.");
                    return null;
                }

                string serializedToken = Serializations.SerializeObj(new UserSession()
                { Token = result.Token, RefreshToken = result.RefreshToken });
                await localStorageService.SetToken(serializedToken);
                return result.Token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred while refreshing token: {ex.Message}");
                return null;
            }
        }
    }
}
