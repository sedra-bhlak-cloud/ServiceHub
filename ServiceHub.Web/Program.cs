using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Web.Services;
using ServiceHub.Web.Data; 

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTER THE DB CONTEXT FIRST

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// 2. REGISTER IDENTITY (It now correctly finds ApplicationDbContext)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 3. REGISTER THE SERVICE LAYER
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
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
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await DbSeeder.SeedData(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred migrating or seeding the DB.");
    }
}
app.UseStaticFiles();
app.Run();