using SpyFall_project.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// builder.Services.AddDbContext<SpyFallDBcontext>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configure the DbContext to use PostgreSQL
builder.Services.AddDbContext<SpyFallDBcontext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.Urls.Add("http://0.0.0.0:80");

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
