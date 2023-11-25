using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using WebApplication1.BackgroundServices;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();

services.AddSwaggerGen();
services.AddCors();
services.AddFileServices(builder.Configuration);

services.AddHostedService<VideoEditingBackgroundService>()
    .AddSingleton(_ => Channel.CreateUnbounded<EditVideoMessage>());

services.AddDbContext<AppDbContext>(config =>
config.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.MapControllers();
app.UseStaticFiles();

app.Run();
