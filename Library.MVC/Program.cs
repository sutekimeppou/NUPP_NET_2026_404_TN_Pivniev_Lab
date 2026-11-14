using Library.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite($"Data Source={LibraryDatabase.GetPath()}"));

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    LibraryContext context = scope.ServiceProvider.GetRequiredService<LibraryContext>();

    await context.Database.MigrateAsync();
    await LibrarySeed.InitializeAsync(context);
}

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseRouting();

app.MapStaticAssets();
app.MapControllerRoute(name: "default", pattern: "{controller=Members}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
