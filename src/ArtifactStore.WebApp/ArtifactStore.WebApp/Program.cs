using ArtifactStore.WebApp.Client.ViewModel.Auth;
using ArtifactStore.WebApp.Client.Pages;
using ArtifactStore.WebApp.Components;
using ArtifactStore.WebApp.Endpoints;
using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Middleware;
using ArtifactStore.WebApp.Services;
using ArtifactStore.WebApp.Shared.Http;
using Blazing.Mvvm;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.AccessDeniedPath = "/Error/forbidden";
        options.LoginPath = "/auth/login";
        
        options.Cookie.Name = "auth.cookie";
        options.Cookie.Path = "/";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMvvm(options =>
{
    options.HostingModelType = BlazorHostingModelType.WebApp;
    options.RegisterViewModelsFromAssemblyContaining<LoginViewModel>();
});

builder.Services.AddScoped<IErrorHandler, ErrorHandler>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IArtifactService, ArtifactService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStoreTransactionService, StoreTransactionService>();
builder.Services.AddScoped(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration.GetRequiredSection("Url:WebApp").Value!)
    };
});

builder.Services.AddHttpClient(HttpClientName.Main, client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetRequiredSection("Url:Main").Value!);
});

builder.Services.AddHttpClient(HttpClientName.Auth, client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetRequiredSection("Url:Auth").Value!);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ArtifactStore.WebApp.Client._Imports).Assembly);

// Endpoints
app.MapAuthEndpoints();
app.MapCharacterEndpoints();
app.MapBalanceEndpoints();
app.MapInventoryEndpoints();
app.MapStoreEndpoints();

app.Run();
