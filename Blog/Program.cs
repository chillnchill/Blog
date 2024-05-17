using Blog.Data;
using Blog.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<BlogDbContext>(options =>
	options.UseSqlServer(connectionString));


// You need the nuget packages for DefaultIdentity (EFC Identity + ASP.NET UI)
builder.Services.AddDefaultIdentity<IdentityUser>()
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<BlogDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IRepository, Repository>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
