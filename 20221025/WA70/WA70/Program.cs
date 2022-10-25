using Northwind.Store.Data;
using WA70;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NWContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
//builder.Services.AddDistributedMemoryCache();
// SQL Server, Redis, NCache
builder.Services.AddSession(options =>
{
    // Set a short timeout for easy testing. The default is 20 minutes.
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    // Make the session cookie essential
    options.Cookie.IsEssential = true;
});
builder.Services.AddTransient(typeof(SessionSettings)); // Por instancia
//builder.Services.AddScoped(typeof(SessionSettings)); // Por solicitud
//builder.Services.AddSingleton(typeof(SessionSettings)); // Solo una instancia

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Items["StartTime"] = $"Pipeline start {DateTime.Now}";
    //throw new ApplicationException("Ooops");
    await next.Invoke();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
