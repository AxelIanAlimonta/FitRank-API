using Microsoft.EntityFrameworkCore;
using FitRank_API.Infrastructure.Persistence;
using System;
using FitRank_API.Application.Services;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Repositories;
using FitRank_API.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FitRankDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPersonaRepository, PersonaRepositoryImpl>();
builder.Services.AddScoped<IPersonaService, PersonaServiceImpl>();
builder.Services.AddScoped<ILogroService, LogroServiceImpl>();
builder.Services.AddScoped<ILogroRepositorio, LogroRepositorio>();
builder.Services.AddScoped<ISocioService, SocioServiceImpl>();
builder.Services.AddScoped<ISocioRepositorio, SocioRepositorio>();
builder.Services.AddScoped<IGimnasioService, GimnasioServiceImpl>();
builder.Services.AddScoped<IGimnasioRepositorio, GimnasioRepositorio>();




builder.Services.AddAutoMapper(cfg =>
   cfg.AddMaps(typeof(FitRank_API.Application.Mappings.AssemblyMapping).Assembly));


builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p
        .WithOrigins("http://localhost:4200") // front
        .AllowAnyHeader()
        .AllowAnyMethod());
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FitRankDbContext>();
    db.Database.Migrate();
}

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
