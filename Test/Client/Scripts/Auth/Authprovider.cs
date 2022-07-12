using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Test.Shared.Services.Auth;
using Microsoft.JSInterop;
using Test.Client.Scripts;

namespace Test.Client.Scripts.Auth
{
    public class Authprovider : IAuthService
    {
        public enum LoginMode
        {
            Mictrosoft = 0,
            Google
        }

        public LoginMode loginMode;
        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            if(((int)loginMode) == 0)
            {
                return await MicrosoftLogin(loginModel);
            }else if(((int)loginMode) == 1)
            {
                return await GoogleLogin(loginModel);
            }
            else
            {
                throw new Exception("Unexpect Mode");
            }
        }

        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public Authprovider(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<LoginResult> MicrosoftLogin(LoginModel loginModel)
        {
            if (loginModel == null)
            {
                throw new ArgumentException("loginModel is null");
            }else if (loginModel == null)
            {
                throw new ArgumentException("loginModel is null");
            }
            else if (loginModel == null)
            {
                throw new ArgumentException("loginModel is null");
            }
            else if (loginModel == null)
            {
                throw new ArgumentException("loginModel is null");
            }
                var token = new CancellationTokenSource().Token;
            bool recive = false;
            string name = string.Empty;
            string address = string.Empty;
            Shared.hubConnection.On<string>("LoginCorect", (messege) =>
             {
                 address = messege.Split(",")[0];
                 name = messege.Split(",")[1];
                 recive = true;
             }
            );
            await JSRuntimeExtensions.InvokeVoidAsync(
                Shared.runtime, 
                "Login", 
                new string[2] {Shared.navigationManager.ToAbsoluteUri("/Loginhub").ToString(),Shared.hubConnection.ConnectionId});
            await 
            /*else
            {
                return new LoginResult()
                {
                    IsSuccessful = false,
                    Error = new AuthenticationException("NotAuthrized")
                };
            }*/
        }

        public async Task<LoginResult> GoogleLogin(LoginModel loginModel)
        {
            // wait 3 seconds
            await Task.Delay(3000);
            if (loginModel == null)
            {
                throw new ArgumentException("loginModel is null");
            }
            if (loginModel.UserID == "demo" && loginModel.Password == "demo")
            {
                var res = new LoginResult()
                {
                    IsSuccessful = true,
                    IDToken = "hoge"
                };
                var roles = new List<string>();
                roles.Add("Admin");
                await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.UserID, res.IDToken, roles);
                return res;
            }
            else if (loginModel.UserID == "demo2" && loginModel.Password == "demo2")
            {
                var res = new LoginResult()
                {
                    IsSuccessful = true,
                    IDToken = "hoge"
                };
                var roles = new List<string>();
                roles.Add("User");
                await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.UserID, res.IDToken, roles);
                return res;
            }
            else
            {
                return new LoginResult()
                {
                    IsSuccessful = false,
                    Error = new AuthenticationException("NotAuthrized")
                };
            }
        }

        public async Task LogoutAsync()
        {
            await ((SPAAuthticateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
