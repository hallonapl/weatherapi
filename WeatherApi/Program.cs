using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherApi.Client;
using WeatherApi.Configuration;
using WeatherApi.Extension;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
	
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<StorageSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("StorageSettings").Bind(settings);
                });
        services.AddOptions<WeatherClientSettings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("WeatherClientSettings").Bind(settings);
                });
        var storageAccountConnectionString = context.Configuration.GetConnectionString("StorageAccount") ?? throw new Exception("StorageAccount connection string is missing");
        services.AddServices(storageAccountConnectionString);
        services.AddHttpClient(nameof(WeatherClient), options =>
        {
            var baseUrl = context.Configuration.GetValue<string>("WeatherClientSettings:BaseUrl") ?? throw new Exception("BaseUrl is missing");
            options.BaseAddress = new Uri(baseUrl);
        });
        services.AddLogging();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
