using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Serilog;

namespace Sach.Services;

public interface IConfigurationService
{
    string ProxyAddress { get; }
    int ProxyPort { get; }
    bool ProxyEnabled { get; }
    string ApiServerUrl { get; }
    int OpacityAnimationSeconds { get; }
    int SearchDelayMilliseconds { get; }
    int ApiRetryDelaySeconds { get; }
    int ApiMaxRetries { get; }
}

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService()
    {
        try
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = configBuilder.Build();
            LoggingService.Logger.Information("Configuration loaded from appsettings.json");
        }
        catch (Exception ex)
        {
            LoggingService.Logger.Warning(ex, "Failed to load configuration. Using defaults");
            _configuration = new ConfigurationBuilder().Build();
        }
    }

    public string ProxyAddress => _configuration["Proxy:Address"] ?? "127.0.0.1";
    public int ProxyPort => int.TryParse(_configuration["Proxy:Port"], out var port) ? port : 12334;
    public bool ProxyEnabled => bool.TryParse(_configuration["Proxy:Enabled"], out var enabled) ? enabled : true;
    public string ApiServerUrl => _configuration["ApiServer:Url"] ?? "http://localhost:5000";
    public int OpacityAnimationSeconds => int.TryParse(_configuration["Timeouts:OpacityAnimationSeconds"], out var seconds) ? seconds : 2;
    public int SearchDelayMilliseconds => int.TryParse(_configuration["Timeouts:SearchDelayMilliseconds"], out var ms) ? ms : 2000;
    public int ApiRetryDelaySeconds => int.TryParse(_configuration["Timeouts:ApiRetryDelaySeconds"], out var delay) ? delay : 20;
    public int ApiMaxRetries => int.TryParse(_configuration["Timeouts:ApiMaxRetries"], out var retries) ? retries : 5;
}

