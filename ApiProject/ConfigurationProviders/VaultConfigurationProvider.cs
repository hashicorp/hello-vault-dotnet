using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
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
            
            string wrappingToken = File.ReadAllText(_vaultSettings.TokenPath); // placed here by a trusted orchestrator
            
            // We need to create two VaultClient objects for authenticating via AppRole. The first is for
            // using the unwrap utility. We need to initialize the client with the wrapping token.
            IAuthMethodInfo wrappedTokenAuthMethod = new TokenAuthMethodInfo(wrappingToken);
            var vaultClientSettingsForUnwrapping = new VaultClientSettings(_vaultSettings.Address, wrappedTokenAuthMethod);

            IVaultClient vaultClientForUnwrapping = new VaultClient(vaultClientSettingsForUnwrapping);

            // We pass null here instead of the wrapping token to avoid depleting its single usage
            // given that we already initialized our client with the wrapping token
            Secret<Dictionary<string, object>> secretIdData =  vaultClientForUnwrapping.V1.System
                .UnwrapWrappedResponseDataAsync<Dictionary<string, object>>(null).Result; 

            var secretId = secretIdData.Data["secret_id"]; // Grab the secret_id 

            AppRoleAuthMethodInfo authMethodInfo = new AppRoleAuthMethodInfo(_vaultSettings.RoleId, secretId.ToString());
            VaultClientSettings clientSettings = new VaultClientSettings(
                _vaultSettings.Address,
                authMethodInfo
            );
        
            _vaultClient = new VaultClient(clientSettings);
        }

        public override void Load()
        {
            Secret<SecretData> secrets = _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "/secrets/api", null).Result;
            string apiKey = secrets.Data.Data["api-key"].ToString();
            Data.Add("api:apikey", apiKey);
        }
    }
}