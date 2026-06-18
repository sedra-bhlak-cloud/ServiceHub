using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Web.Services; // <-- Added this namespace to find your new service files

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTER THE DB CONTEXT FIRST
// Ensure you have a connection string named "DefaultConnection" in appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. REGISTER IDENTITY (It now correctly finds ApplicationDbContext)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 3. REGISTER THE SERVICE LAYER
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>(); // <-- Added your service registration line here!

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ServiceRequest}/{action=Index}/{id?}");

app.MapRazorPages(); 

// Seeding logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ServiceHub.Web.Data.DbSeeder.SeedData(services);
}
app.UseStaticFiles();
app.Run();