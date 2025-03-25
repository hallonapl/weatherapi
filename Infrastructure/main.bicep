targetScope = 'subscription'

module storage 'Storage/main.bicep' = {
  name: 'storage'
  scope: resourceGroup('temp')
  params: {
    storageAccountName: 'temp'
    kind: 'StorageV2'
    skuName: 'Standard_LRS'
  }
}
