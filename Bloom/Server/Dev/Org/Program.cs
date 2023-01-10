using Bloom.Server.Dev.Org;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddAuthentication()
   .AddMicrosoftAccount(microsoftOptions =>
   {
       var mid = builder.Configuration["Authentication:Microsoft:ClientId"];
       var ms = builder.Configuration["Authentication:Microsoft:ClientSecret"];
       if (mid != null && ms != null)
       {
           microsoftOptions.ClientId = mid;
           microsoftOptions.ClientSecret = ms;
           Console.WriteLine("Add Microsoft Authentication provider");
       }
       else
       {
           Console.WriteLine("Cannot Add Microsoft Authentication provider");
       }
   }).AddGoogle(options =>
       {
           var gc = builder.Configuration.GetSection("Authentication:Google");
           if (gc != null)
           {
               IConfigurationSection googleAuthNSection = gc;
               options.ClientId = googleAuthNSection["ClientId"];
               options.ClientSecret = googleAuthNSection["ClientSecret"];
                Console.WriteLine("Add Google Authentication provider");
           }
           else
           {
                Console.WriteLine("Cannot Add Google Authentication provider");
           }
   });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
