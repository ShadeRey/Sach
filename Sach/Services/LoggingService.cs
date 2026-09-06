using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Sach.Services;

public static class LoggingService
{
    private static ILogger? _logger;

    public static ILogger Logger => _logger ??= CreateLogger();

    private static ILogger CreateLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                "logs/sach-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            .CreateLogger();
    }

    public static void Close()
    {
        Log.CloseAndFlush();
    }
}
