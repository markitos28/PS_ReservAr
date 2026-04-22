using Microsoft.EntityFrameworkCore;
using ReservAr.Data;
using ReservAr.Services;
using ReservAr.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ReservArDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();

// 🔥 IMPORTANTE
builder.Services.AddControllers();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ISectorService, SectorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// archivos estáticos (login.html)
app.UseStaticFiles();

// redirigir al login
app.MapGet("/", context =>
{
    context.Response.Redirect("/login.html");
    return Task.CompletedTask;
});

// 🔥 controllers
app.MapControllers();

app.Run();