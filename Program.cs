using Microsoft.EntityFrameworkCore;
using WebUygulama1.Models;
using WebUygulama1.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// UygulamaDbContext i servis olarak bildirmemiz gerekiyor.
builder.Services.AddDbContext<UygulamaDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<UygulamaDbContext>()
                                            .AddDefaultTokenProviders();
builder.Services.AddRazorPages();

// DIKKAT! Yeni bir Repository s�n�f� olu�turdu�umuzda mutlaka burada Serrvices'lere eklemeliyiz.
// _kitapTuruRepository nesnesi => Dependency Injection
builder.Services.AddScoped<IKitapTuruRepository, KitapTuruRepository>();

// _kitapRepository nesnesi => Dependency Injection
builder.Services.AddScoped<IKitapRepository, KitapRepository>();

// _kiralamaRepository nesnesi => Dependency Injection
builder.Services.AddScoped<IKiralamaRepository, KiralamaRepository>();

builder.Services.AddScoped<IEmailSender, EmailSender>();


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

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
