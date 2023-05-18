using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration
    .AddJsonFile("ocelot.json", false, true)
    .Build();

builder.Services.AddCors();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseCors(e => {
    e.AllowAnyOrigin();
    e.AllowAnyMethod();
    e.AllowAnyHeader();
});

app.UseOcelot().Wait();

app.Run();
