using MassTransit;
using Serilog;
using Serilog.Events;
using SmartApp.Infrastructure.Extensions;
using SmartApp.Worker.Consumers;

using MicrosoftHost = Microsoft.Extensions.Hosting.Host;
using IHostBuilder = Microsoft.Extensions.Hosting.IHostBuilder;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting SmartApp.Worker");

    IHostBuilder hostBuilder = MicrosoftHost.CreateDefaultBuilder(args)
        .UseSerilog((ctx, services, lc) => lc
            .ReadFrom.Configuration(ctx.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/worker-.log",
                rollingInterval: Serilog.RollingInterval.Day,
                retainedFileCountLimit: 30))
        .ConfigureServices((ctx, services) =>
        {
            services.AddInfrastructureServices(ctx.Configuration);

            services.AddMassTransit(cfg =>
            {
                cfg.SetKebabCaseEndpointNameFormatter();

                cfg.AddConsumer<GenerateEmployeeReportConsumer>(c =>
                {
                    c.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(30)));
                });

                cfg.UsingRabbitMq((busCtx, rmq) =>
                {
                    rmq.Host(
                        ctx.Configuration["RabbitMq:Host"]!,
                        ctx.Configuration["RabbitMq:VHost"] ?? "/",
                        h =>
                        {
                            h.Username(ctx.Configuration["RabbitMq:Username"]!);
                            h.Password(ctx.Configuration["RabbitMq:Password"]!);
                        });

                    rmq.ConfigureEndpoints(busCtx);
                });
            });
        });

    await hostBuilder.Build().RunAsync();
}
catch (Exception ex) when (ex is not OperationCanceledException && ex.GetType().Name != "StopTheHostException")
{
    Log.Fatal(ex, "SmartApp.Worker terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}