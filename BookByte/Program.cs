using BookByte.DataAccess.Data;
using BookByte.DataAccess.Repository;
using BookByte.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BookByte.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using BookByte.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//add this after adding identity(above line)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

//facebook authentication
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "1173297387373601";
    options.AppSecret = "4cf6320e3d6eebc183388ed47d4e5ca6";
});

//session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
    

//To tell the application we also have razor pages for Identity.
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

//DbInitializer
builder.Services.AddScoped<IDbInitializer,DbInitializer>();

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

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();
//checks if username & password is valid
app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

//call seedDatabase
SeedDatabase();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
