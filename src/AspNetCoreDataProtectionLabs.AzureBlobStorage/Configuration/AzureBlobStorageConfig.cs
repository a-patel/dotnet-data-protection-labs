namespace AspNetCoreDataProtectionLabs.AzureBlobStorage.Configuration
{
    public class AzureBlobStorageConfig
    {
        public string StorageAccountBlobBaseUrl { get; set; }

        public string StorageContainerName { get; set; }

        public string StorageBlobName { get; set; }

        public string SharedTokenCacheTenantId { get; set; }
    }
}
