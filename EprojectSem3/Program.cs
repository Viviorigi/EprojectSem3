using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using BussinessLogicLayer_BLL.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserService>();
builder.Services.AddScoped<ICategoryRepository, CategoryService>();
builder.Services.AddScoped<IListingRepository, ListingService>();
builder.Services.AddScoped<IRegionRepository, RegionService>();
builder.Services.AddScoped<ICityRepository, CityService>();

builder.Services.AddAuthentication(options =>
{
    // Set default scheme (optional)
    options.DefaultScheme = "MyAuthenticationSchema";
})
.AddCookie("MyAuthenticationSchema", options =>
{
    options.Cookie = new CookieBuilder
    {
        Name = "My.Authentication.Schema",
        Path = "/",
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.SameAsRequest
    };
    options.LoginPath = new PathString("/Home/Login");
    options.SlidingExpiration = true;
})
.AddCookie("MyAppAuthenticationAdmin", options =>
{
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = "MyApp.Authentication.Admin",
        Path = "/",
        SameSite = SameSiteMode.Lax,
        SecurePolicy = CookieSecurePolicy.SameAsRequest
    };
    options.LoginPath = new PathString("/Admin/Login/Index");
    options.LogoutPath = new PathString("/Admin/Login/Logout");
});

builder.Services.AddTransient<EmailService>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(

    name: "Areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
