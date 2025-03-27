using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherApi.Configuration;
using WeatherApi.Extension;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
	
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<StorageSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("BlobStorageSettings").Bind(settings);
                });
        services.AddServices(context.Configuration.GetConnectionString("StorageAccount"));
        services.AddLogging();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
