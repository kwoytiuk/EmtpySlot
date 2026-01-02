using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EmptySlot.Web;
using EmptySlot.Web.Services;
using EmptySlot.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<EmptySlot.Shared.UI.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient and API service
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000/") });
builder.Services.AddScoped<IApiService, WebApiService>();

await builder.Build().RunAsync();
