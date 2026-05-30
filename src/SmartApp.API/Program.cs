using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartApp.API.Middleware;
using SmartApp.Application.Extensions;
using SmartApp.Infrastructure.Extensions;
using SmartApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30));

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();

    cfg.UsingRabbitMq((ctx, rmq) =>
    {
        rmq.Host(
            builder.Configuration["RabbitMq:Host"]!,
            builder.Configuration["RabbitMq:VHost"] ?? "/",
            h =>
            {
                h.Username(builder.Configuration["RabbitMq:Username"]!);
                h.Password(builder.Configuration["RabbitMq:Password"]!);
            });

        rmq.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "SmartApp API", Version = "v1", Description = "Employee Report Generator via RabbitMQ" }));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartApp API v1"));
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
