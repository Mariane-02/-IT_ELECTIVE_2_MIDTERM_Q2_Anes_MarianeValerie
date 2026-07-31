var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// 1. Add Session Services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// 2. Add Session Middleware (Must be before MapControllerRoute)
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
