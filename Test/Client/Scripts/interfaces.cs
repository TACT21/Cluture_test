using Microsoft.JSInterop;
using Microsoft.AspNetCore.SignalR.Client;
namespace Test.Client.Scripts
{
    public class Interfaces
    {
        static Interfaces interfaces = new Interfaces();
        public JSRuntime runtime;
        public HubConnection hubConnection;
    }
}
