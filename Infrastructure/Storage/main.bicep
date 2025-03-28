@description('The name of the storage account')
param storageAccountName string

@description('The SKU of the storage account')
param skuName string

@description('The kind of the storage account')
param kind string

@description('The weather blob container name')
param blobContainerName string

@description('The log table name')
param tableName string

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

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2024-01-01' = {
  name: 'default'
  parent: storageAccount
  properties: {}
}

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2024-01-01' = {
  name: blobContainerName
  parent: blobService
  properties: {}
}

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2024-01-01' = {
  name: 'default'
  parent: storageAccount
  properties: {}
}

resource table 'Microsoft.Storage/storageAccounts/tableServices/tables@2024-01-01' = {
  name: tableName
  parent: tableService
  properties: {}
}

output storageAccountId string = storageAccount.id
