using Library.Common.Interfaces;
using Library.Common.Models;
using Library.Infrastructure;
using Library.Infrastructure.Mapping;
using Library.Infrastructure.Models;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite($"Data Source={LibraryDatabase.GetPath()}"));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEntityMapper<Book, BookModel>, BookMapper>();
builder.Services.AddScoped<IEntityMapper<Member, MemberModel>, MemberMapper>();
builder.Services.AddScoped<ICrudServiceAsync<Book>, DatabaseCrudService<Book, BookModel>>();
builder.Services.AddScoped<ICrudServiceAsync<Member>, DatabaseCrudService<Member, MemberModel>>();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    LibraryContext context = scope.ServiceProvider.GetRequiredService<LibraryContext>();

    await context.Database.MigrateAsync();
    await LibrarySeed.InitializeAsync(context);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
