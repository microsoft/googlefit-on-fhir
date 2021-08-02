@description('Base name that is used to name provisioned resources. Should be alphanumeric and less than 16 characters.')
@minLength(3)
@maxLength(16)
param basename string = 'fitonfhir'

resource usersKeyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: 'kv-users-${basename}'
  location: resourceGroup().location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        //TODO: this is a placeholder policy that is required for provisioning but should be removed when there is a managed identity that can be used
        tenantId: subscription().tenantId
        objectId: subscription().tenantId
        permissions: {
          secrets: [
            'all'
          ]
        }
      }
    ]
    tenantId: subscription().tenantId
    enableSoftDelete: true
    enablePurgeProtection: true
    softDeleteRetentionInDays: 30
  }
}

resource infraKeyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: 'kv-infra-${basename}'
  location: resourceGroup().location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        //TODO: this is a placeholder policy that is required for provisioning but should be removed when there is a managed identity that can be used
        tenantId: subscription().tenantId
        objectId: subscription().tenantId
        permissions: {
          secrets: [
            'all'
          ]
        }
      }
    ]
    tenantId: subscription().tenantId
    enableSoftDelete: true
    enablePurgeProtection: true
    softDeleteRetentionInDays: 30
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: replace('sa-${basename}', '-', '')
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}

resource queueService 'Microsoft.Storage/storageAccounts/queueServices@2021-04-01' = {
  parent: storageAccount
  name: 'default'
}

resource queue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  parent: queueService
  name: 'google-fit-users'
  properties: {
    metadata: {}
  }
  dependsOn: [
    storageAccount
  ]
}

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${infraKeyVault.name}/queue-connection-string'
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
  }
}

output usersKeyVaultName string = usersKeyVault.name
output infraKeyVaultName string = infraKeyVault.name
