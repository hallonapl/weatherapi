@description('The name of the storage account')
param storageAccountName string

@description('The SKU of the storage account')
param skuName string

@description('The kind of the storage account')
param kind string

var location = resourceGroup().location

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: skuName
  }
  kind: kind
  properties: {}
}

output storageAccountId string = storageAccount.id
