using Microsoft.AspNetCore.ResponseCompression;
using Bloom.Server.Hubs;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddResponseCompression();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();
app.UseResponseCompression();
// to reverce proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
// to reverce proxy End

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapHub<Exploer>("/maphub");
Console.WriteLine("add!");
app.MapHub<Company>("/grouphub");
app.MapHub<Bloom.Server.Controllers.Vote>("/votehub");

app.MapFallbackToFile("index.html");

app.Run();

var nowait = Unwait();

async Task Unwait()
{
    Console.ReadLine();
    Console.Write("Are you sure close?");
    if(Console.ReadLine() == "y")
    {
        Environment.Exit(0);
    }
}