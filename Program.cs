using Blog.Data;
using Blog.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Blog.Extensions;
using Blog.Data.FileManager;
using AspNetCoreHero.ToastNotification;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(connectionString));



// You need the nuget packages for DefaultIdentity (EFC Identity + ASP.NET UI)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
})
   // .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BlogDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";
});

builder.Services.AddControllersWithViews();
builder.Logging.AddConsole();
builder.Services.AddTransient<IRepository, Repository>();
builder.Services.AddTransient<IFileManager, FileManager>();
builder.Services.AddNotyf(config =>
{
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");    
    app.UseHsts();
}

app.UseHttpsRedirection();
//this is to load images (for example)
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.SeedAdministrator("admin@blog.com");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
