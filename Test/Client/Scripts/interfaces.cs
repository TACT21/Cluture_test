using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace Test.Client.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginModel loginModel, int mode, Logintools tools);
        Task LogoutAsync();
    }

    public class LoginModel
    {
        public string UserID { get; set; }

        public string Password { get; set; }
    }

    public class LoginResult
    {
        public bool IsSuccessful { get; set; }
        public Exception Error { get; set; }
        public string IDToken { get; set; }
        public string name { get; set; }
        public string address { set; get; }
        public byte[] avater { set; get; }
    }

    public class Logintools
    {
        public NavigationManager nav { get; set; }
        public IJSRuntime jS { get; set; }
    }
}