# Example usage: .\TestBicep.ps1 -bicepFilePath '..\main.bicep' -parameterFilePath '..\Configuration\main.bicepparam'
# Define variables
param(
    [Parameter(Mandatory=$true)][string]$bicepFilePath,
    [Parameter(Mandatory=$true)][string]$parameterFilePath
)
$resourceGroupName = "weatherapi-rg"

# Run the what-if deployment
az deployment group what-if --resource-group $resourceGroupName --template-file $bicepFilePath --parameters $parameterFilePath