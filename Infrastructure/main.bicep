@description('The name of the storage account')
param storageAccountName string

targetScope = 'resourceGroup'

module storage 'Storage/main.bicep' = {
  name: 'storage'
  params: {
    storageAccountName: storageAccountName
    kind: 'StorageV2'
    skuName: 'Standard_LRS'
    blobContainerName: 'weatherdata'
    tableName: 'weatherlogs'
  }
}

output storageAccountId string = storage.outputs.storageAccountId
