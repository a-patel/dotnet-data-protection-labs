#region Imports
using AspNetCoreDataProtectionLabs.AzureKeyVault.Configuration;
using AspNetCoreDataProtectionLabs.AzureKeyVault.Extensions;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#endregion

namespace AspNetCoreDataProtectionLabs.AzureKeyVault
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
            var azureKeyVaultConfig = Configuration.GetSection("AzureKeyVault").Get<AzureKeyVaultConfig>();
            var keyIdentifier = azureKeyVaultConfig.KeyVaultKeyId;
            var tokenCredential = GetTokenCredential(azureKeyVaultConfig);

            // Important NOTE: Key Vault is only for protection, one more layer of extra security
            // Key Vault is not used to PersistKeys 
            // Need to add other storage like Blob storage, Redis etc. to PersistKeys
            // Key Vault requires the user/app to have permissions on Keys: 1. Read 2. Wrap key 3.Unwrap key
            services.AddDataProtection()
                .ProtectKeysWithAzureKeyVault(keyIdentifier, tokenCredential);

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

        private TokenCredential GetTokenCredential(AzureKeyVaultConfig config)
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



#region Reference
/*
https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview
 */
#endregion
