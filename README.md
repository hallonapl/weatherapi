# Weather API

## About
The Weather API is a simple service that provides weather data for London. It fetches and stores weather conditions, and allows users to allows users to fetch these reports.
It is implemented as a series of Azure Functions.

## Features
- Automatically request and store current weather data.
- See logs for all requests.
- Get specific stored weather data.

## Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/your-username/weather-api.git
    ```
2. Sign in to Azure
    ```bash
    az login
    ```
3. If necessary, create resource group for the application
    ```bash
    az group create --location <Location> --name <ResourceGroupName>
    ```
4. Deploy the infrastructure using the provided Bicep script:
    ```bash
    az deployment group create --resource-group <ResourceGroupName> --template-file Infrastructure/main.bicep --parameters Infrastructure/Configuration/main.bicepparam
    ```
    Replace `<ResourceGroupName>` with the name of your Azure resource group.
5. Open the WeatherApi/WeatherApi.sln solution in Visual Studio.
6. Replace values in local.settings.json:
    i. "WeatherClientSettings:ApiKey": <ApiKeyForOpenWeatherMap>,
    ii. "ConnectionStrings:StorageAccount": <StorageAccountConnectionString>
    `<StorageAccountConnectionString>` is the connection string for the newly created storage account `weatherapistorage` in Azure Portal.

## Usage
1. Start the solution in Visual Studio.

## Endpoints
- `GET http://localhost:7207/api/logs`: Fetch logs for all requests.
- `GET http://localhost:7207/api/logs/<id>`: Fetch weather report with id <id>
