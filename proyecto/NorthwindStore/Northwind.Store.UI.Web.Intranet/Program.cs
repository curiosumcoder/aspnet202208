using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Store.Data;
using Northwind.Store.Model;
using Northwind.Store.UI.Web.Intranet.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("NW");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
//builder.Services.AddDbContext<NWContext>(options =>
//    options.UseSqlServer(connectionString));
builder.Services.AddDbContextPool<NWContext>(options =>
{
#if DEBUG
    //options.LogTo(Console.WriteLine);
#endif
    options.UseSqlServer(connectionString);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddMemoryCache();
//builder.Services.AddDistributedMemoryCache();
// SQL Server, Redis, NCache
builder.Services.AddSession(options =>
{
    // Set a short timeout for easy testing. The default is 20 minutes.
    options.IdleTimeout = TimeSpan.FromSeconds(5);
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "YourAppCookieName";
    options.Cookie.HttpOnly = true;
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
    options.LoginPath = "/Identity/Account/Login";
    // ReturnUrlParameter requires 
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    //options.SlidingExpiration = false;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
//https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.mvcoptions.suppressimplicitrequiredattributefornonnullablereferencetypes?view=aspnetcore-6.0
//builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

#region Autorización
// Requerir autenticaci�n para todo el sitio, se except�a
// el uso espec�fico de Authorize o AllowAnonymous. RECOMENDADO    
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
});
#endregion

builder.Services.AddTransient<CategoryRepository>(); // Instancia por controlador
//builder.Services.AddScoped<CategoryRepository>(); // Instancia por request
//builder.Services.AddSingleton<CategoryRepository>(); // �nica instancia para todos
builder.Services.AddTransient<BaseRepository<Product, int>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Status Codes
//app.UseStatusCodePages();
app.UseStatusCodePagesWithRedirects("/Home/ErrorWithCode?code={0}");
//app.UseStatusCodePagesWithReExecute("/Home/ErrorWithCode", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//app.MapAreaControllerRoute("admin", "admin", "admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();


// SQL Injection
// Cross-Site Scripting (XSS)
// Cross-Site Request Forgery (XSRF/CSRF)
// Open Redirect Attacks
// Cross-Origin Requests (CORS)

// Logging Levels: Trace = 0, Debug = 1, Information = 2 (*Default), Warning = 3, Error = 4, Critical = 5, and None = 6.	
//
// Providers: Console, Debug, *EventSource, EventLog	