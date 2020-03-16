#region Imports
using AspNetCoreDataProtectionLabs.AzureBlobStorage.Configuration;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using AspNetCoreDataProtectionLabs.AzureBlobStorage.Extensions;

#endregion

namespace AspNetCoreDataProtectionLabs.AzureBlobStorage
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var azureBlobStorageConfig = Configuration.GetSection("AzureBlobStorage").Get<AzureBlobStorageConfig>();
            var blobClient = GetBlobClient(azureBlobStorageConfig);

            // Blob Storage requires the user/app to have: Storage Blob Data Contributor role at Storage account or container level
            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(blobClient);

            services.AddCustomSwagger();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCustomSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region Utilities

        private BlobClient GetBlobClient(AzureBlobStorageConfig config)
        {
            var tokenCredential = GetTokenCredential(config);
            var client = new BlobServiceClient(new Uri(config.StorageAccountBlobBaseUrl), tokenCredential);

            var containerClient = client.GetBlobContainerClient(config.StorageContainerName);
            var blobClient = containerClient.GetBlobClient(config.StorageBlobName);

            return blobClient;
        }

        private TokenCredential GetTokenCredential(AzureBlobStorageConfig config)
        {
            var credentialOptions = new DefaultAzureCredentialOptions();

            if (config.SharedTokenCacheTenantId != null)
            {
                credentialOptions.SharedTokenCacheTenantId = config.SharedTokenCacheTenantId;
            }

            return new DefaultAzureCredential(credentialOptions);
        }

        #endregion
    }
}
