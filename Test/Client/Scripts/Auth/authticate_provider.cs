using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Test.Client.Scripts.Auth
{
    public class SPAAuthticateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public SPAAuthticateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            var userID = await _localStorage.GetItemAsync<string>("userID");
            var roles = await _localStorage.GetItemAsync<List<string>>("roles");
            // do token check if it is needed

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, userID));

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // HTTPの認証用のトークンを設定
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "User")));
        }

        public async Task MarkUserAsAuthenticated(string userID, string authToken, List<string> roles = null)
        {
            await _localStorage.SetItemAsync("userID", userID);
            await _localStorage.SetItemAsync("authToken", authToken);
            if (roles != null)
            {
                await _localStorage.SetItemAsync("roles", roles);
            }
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync("userID");
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("roles");
            if (_httpClient.DefaultRequestHeaders.Authorization != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}