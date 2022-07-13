using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Test.Client.Scripts;
using Test.Client.Services.Auth;

namespace Test.Client.Scripts.Auth
{
    public class Authprovider : IAuthService
    {
        public Dictionary<string, int> LoginMode = new Dictionary<string, int>()
        {
            {"Microsoft", 0},
            {"Google", 1}
        };
        public async Task<LoginResult> LoginAsync(LoginModel loginModel, int login,Logintools tools)
        {
            LoginResult result;
            if (login== 0)
            {
                try
                {
                    result =  await MicrosoftLogin(loginModel, tools);
                }catch(Exception e)
                {
                    throw;
                }
                return result;
            }else if(login == 1)
            {
                try { 
                    result =  await GoogleLogin(loginModel);
                }
                catch (Exception e)
                {
                    throw;
                }
                return result;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginModel"></param>
        /// <param name="tools"></param>
        /// <param name="wait"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<LoginResult> MicrosoftLogin(LoginModel loginModel ,Logintools tools,int wait = 30000,int unit = 500)
        {
            if (loginModel == null)
            {
                throw new ArgumentException("loginModel is null");
            }else if (tools == null)
            {
                throw new ArgumentException("tools is null");
            }
                var token = new CancellationTokenSource().Token;
            bool recive = false;
            LoginResult result = new LoginResult();
            await JSRuntimeExtensions.InvokeVoidAsync(
                tools.jS, 
                "Login", 
                new string[2] {tools.nav.ToAbsoluteUri("/Loginhub").ToString(),tools.hub.ConnectionId});
            tools.hub.On<string>("LoginCorect", async (message) => {
                var a = message.Split(",");
                result.name = a[1];
                result.address = a[0];
                recive = true;
            });
            for (int i = 0; i < (wait/unit); i++)
            {
                await Task.Delay(unit);
                if(recive == true)
                {
                    break;
                }
            }
            if (recive == false)
            {
                result.Error = new TimeoutException();
            }
            return result;
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
