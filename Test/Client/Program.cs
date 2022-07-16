using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Test.Client;
using Test.Client.Scripts.Auth;
using Test.Client.Services.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, SPAAuthticateProvider>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthService, DummyAuthService>();
builder.Services.AddScoped<ILocalStorageAuth, LocalStorageAuthticateProvider>();
builder.Services.AddAuthorizationCore(config =>
{
    config.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin", "SuperUser"));
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
