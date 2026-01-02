using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EmptySlot.Web;
using EmptySlot.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<EmptySlot.Shared.UI.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register API service
// TODO: Implement a web-specific HttpClient-based ApiService
// For now, we'll need to create a WebApiService that uses HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
