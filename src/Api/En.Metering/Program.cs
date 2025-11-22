using En.Metering.Middleware;
using En.Metering.Models.Requests;
using En.Metering.Persistence.Context;
using En.Metering.Persistence.Repositories;
using En.Metering.Persistence.UnitOfWork;
using En.Metering.Services.DTOs;
using En.Metering.Services.Implementations;
using En.Metering.Services.Interfaces;
using En.Metering.Validation.Implementaions;
using En.Metering.Validation.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddDbContext<MeteringDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MeteringDb")));
builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMeterReadingProcessor, MeterReadingProcessor>();
builder.Services.AddScoped<ICsvParserService<MeterReadingDto>, CsvParserService>();
builder.Services.AddScoped<IMeterReadingQueryService, MeterReadingQueryService>();
builder.Services.AddScoped<IAccountValidation, AccountValidation>();
builder.Services.AddScoped<IMeterReadingValidation, MeterReadingValidationService>();
builder.Services.AddScoped<IValidator<UploadMeterReadingsRequest>, UploadMeterReadingsRequestValidator>();
builder.Services.AddScoped<IValidator<MeterReadingDto>, MeterReadingValueValidator>();

// Resolve max degree from config if present
var maxDp = builder.Configuration.GetValue<int?>("Metering:MaxDegreeOfParallelism");

// Register MeterReadingUploadService with IServiceProvider injected and optional max degree
builder.Services.AddScoped<IMeterReadingUploadService>(sp =>
{
    var processor = sp.GetRequiredService<IMeterReadingProcessor>();
    var csv = sp.GetRequiredService<ICsvParserService<MeterReadingDto>>();
    var logger = sp.GetRequiredService<Serilog.ILogger>();
    return new MeterReadingUploadService(processor, csv, logger, sp, maxDp);
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientId,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

app.UseCors("ClientPolicy");
app.MapControllers();
app.UseRateLimiter();

app.Run();
