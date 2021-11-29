using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FrontEnd;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("FrontEnd.ServerAPI", client => client.BaseAddress = new Uri("https://localhost:7021/"))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// https://stackoverflow.com/questions/36656777/azureb2c-identity-provider-login-failed-aadb2c99002-user-does-not-exist
// DEN HER ER LIDT VIGTIG LIGE AT KIGGE PÅ !
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("FrontEnd.ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://coprojectitu.onmicrosoft.com/029a8171-e8c6-49cd-bcce-ad5952ba5ecb/APIAccess");
    options.ProviderOptions.LoginMode = "redirect";
});

await builder.Build().RunAsync();
