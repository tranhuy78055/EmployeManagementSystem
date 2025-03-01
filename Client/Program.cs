using BaseLibrary.Entities;
using Blazored.LocalStorage;
using Client;
using Client.ApplicationStates;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using ClientLibrary.Services.Implementations;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Register Syncfusion license
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWX1ccHRTRWZZV0J1WUI=");

builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7294/");

}).AddHttpMessageHandler<CustomHttpHandler>(); ;
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7294") });
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider,CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IUserAccountService,UserAccountService>();

// General Department/ Department/ Branch
builder.Services.AddScoped<IGnericServiceInterface<GeneralDepartment>, GenericServiceImplementation<GeneralDepartment>>();
builder.Services.AddScoped<IGnericServiceInterface<Department>, GenericServiceImplementation<Department>>();
builder.Services.AddScoped<IGnericServiceInterface<Branch>, GenericServiceImplementation<Branch>>();

// City/ Country/ Town
builder.Services.AddScoped<IGnericServiceInterface<Country>, GenericServiceImplementation<Country>>();
builder.Services.AddScoped<IGnericServiceInterface<Town>, GenericServiceImplementation<Town>>();
builder.Services.AddScoped<IGnericServiceInterface<City>, GenericServiceImplementation<City>>();
// Employee
builder.Services.AddScoped<IGnericServiceInterface<Employee>, GenericServiceImplementation<Employee>>();

builder.Services.AddScoped<AllState>();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<SfDialogService>();

await builder.Build().RunAsync();
