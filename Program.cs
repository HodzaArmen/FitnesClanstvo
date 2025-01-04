using FitnesClanstvo.Data;
using FitnesClanstvo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));
var connectionString = builder.Configuration.GetConnectionString("AzureContext");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FitnesContext>(options =>
            options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FitnesContext>();

var app = builder.Build();

// using(var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<FitnesContext>();
//     DbInitializer.Initialize(context);
// }
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FitnesContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
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
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
