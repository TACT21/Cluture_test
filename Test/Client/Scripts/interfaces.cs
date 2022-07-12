using Microsoft.JSInterop;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
namespace Test.Client.Scripts
{
    public static class Shared
    {
        public static JSRuntime runtime;
        public static HubConnection hubConnection;
        public static NavigationManager navigationManager; 
    }
}
