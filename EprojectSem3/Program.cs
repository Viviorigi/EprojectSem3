using DataAccessLayer_DAL.Models;
using DataAccessLayer_DAL.Repositories;
using BussinessLogicLayer_BLL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;


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
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionService>();
builder.Services.AddScoped<IImageRepository, ImageService>();

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
    options.LoginPath = new PathString("/Admin/LoginAdmin/Index");
    options.LogoutPath = new PathString("/Admin/LoginAdmin/Logout");
});

builder.Services.AddTransient<EmailService>();

//google
builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    SeedAdminUser(context);
}


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


void SeedAdminUser(AppDbContext context)
{
    // Check if admin already exists
    if (!context.Users.Any(u => u.Email == "admin@gmail.com"))
    {
        var admin = new User
        {
            FullName = "Admin User",
            Email = "admin@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"), // Encrypt password
            PhoneNumber = "0123456789",
            Address = "123 Admin St",
            Role = 2, // Admin role
            Status = 1 // Active
        };

        context.Users.Add(admin);
        context.SaveChanges();
    }
}