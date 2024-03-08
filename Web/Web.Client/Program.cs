using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

//builder.Services.AddHttpClient();

builder.Services.AddScoped(sp => new HttpClient
{
    //BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    //BaseAddress = new Uri("https://localhost:5272")
});

builder.Services.AddScoped<ApiService>();

// https://localhost:7043

await builder.Build().RunAsync();
