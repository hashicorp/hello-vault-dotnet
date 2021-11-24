using System;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class VaultConfigurationProvider : ConfigurationProvider
    {    
        private VaultSettings _vaultSettings;
        private VaultClient _vaultClient;

        public VaultConfigurationProvider(VaultSettings vaultSettings)
        {
            _vaultSettings = vaultSettings;
            
            AppRoleAuthMethodInfo authMethodInfo = new AppRoleAuthMethodInfo(_vaultSettings.RoleName, "");
            VaultClientSettings clientSettings = new VaultClientSettings(
                _vaultSettings.Address,
                authMethodInfo
            );
        
            _vaultClient = new VaultClient(clientSettings);
        }

        public override void Load()
        {
            Secret<SecretData> secrets = _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "/creds").Result;
            string apiKey = secrets.Data.Data["api-key"].ToString();
            Data.Add("api:apikey", apiKey);
        }
    }
}