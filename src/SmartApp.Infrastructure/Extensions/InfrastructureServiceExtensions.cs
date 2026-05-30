namespace SmartApp.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartApp.Application.Abstractions;
using SmartApp.Application.Settings;
using SmartApp.Domain.Interfaces.Repositories;
using SmartApp.Domain.Interfaces.Services;
using SmartApp.Infrastructure.Messaging;
using SmartApp.Infrastructure.Persistence;
using SmartApp.Infrastructure.Persistence.Repositories;
using SmartApp.Infrastructure.Services;


public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ReportSettings>(configuration.GetSection(ReportSettings.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(15), errorNumbersToAdd: null);
                    sql.CommandTimeout(60);
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                }));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IExcelReportService, ExcelReportService>();
        services.AddScoped<IMessagePublisher, MassTransitMessagePublisher>();

        return services;
    }
}