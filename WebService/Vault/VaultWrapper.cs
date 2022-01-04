using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace WebService.Vault
{
    public class VaultWrapper
    {
        private readonly ILogger _logger;
        private readonly IVaultClient _client;
        private readonly VaultWrapperSettings _settings;

        public VaultWrapper(ILoggerFactory loggerFactory, VaultWrapperSettings settings)
        {
            _logger = loggerFactory.CreateLogger("Vault");
            _client = AppRoleAuthClient(settings);
            _settings = settings;
        }

        /// <summary>
        /// A combination of a role id and a secret id is required to log into Vault
        /// with AppRole authentication method. The secret id is a value that needs
        /// to be protected, so instead of the app having knowledge of the secret id
        /// directly, we have a trusted orchestrator (simulated with a container)
        /// give the app access to a short-lived response-wrapping token.
        /// </summary>
        ///
        /// <seealso href="https://www.vaultproject.io/docs/concepts/response-wrapping"/>
        /// <seealso href="https://learn.hashicorp.com/tutorials/vault/secure-introduction?in=vault/app-integration#trusted-orchestrator"/>
        /// <seealso href="https://learn.hashicorp.com/tutorials/vault/approle-best-practices?in=vault/auth-methods#secretid-delivery-best-practices"/>
        private IVaultClient AppRoleAuthClient(VaultWrapperSettings settings)
        {
            _logger.LogInformation($"logging in to vault @ { settings.Address } with approle role id { settings.AppRoleAuthRoleId }: started");

            // The wrapping token is placed here by our trusted orchestrator
            string wrappingToken = File.ReadAllText(settings.AppRoleAuthSecretIdFile).Trim();

            // We can't reuse the default VaultClient instance for unwrapping because
            // it needs to be intialized with a different TokenAuthMethodInfo
            IVaultClient vaultClientForUnwrapping = new VaultClient
            (
                new VaultClientSettings(settings.Address, new TokenAuthMethodInfo(vaultToken: wrappingToken))
            );

            // We pass null here instead of the wrapping token to avoid depleting
            // the token's single use. This is to work around the fact that VaultSharp
            // requires a valid wrapping token to initialize the VaultClient.
            string appRoleAuthSecretId
                = vaultClientForUnwrapping.V1.System
                    .UnwrapWrappedResponseDataAsync<Dictionary<string, object>>(tokenId: null)
                        .Result.Data[ "secret_id" ]
                            .ToString();

            AppRoleAuthMethodInfo appRoleAuth = new AppRoleAuthMethodInfo
            (
                roleId: settings.AppRoleAuthRoleId,
                secretId: appRoleAuthSecretId
            );

            IVaultClient client = new VaultClient
            (
                new VaultClientSettings(settings.Address, appRoleAuth)
            );

            _logger.LogInformation($"logging in to vault @ { settings.Address } with approle role id { settings.AppRoleAuthRoleId }: done");

            return client;
        }

        public string GetSecretApiKey()
        {
            _logger.LogInformation("getting secret api key from vault: started");

            Secret<SecretData> secret = _client.V1.Secrets.KeyValue.V2.ReadSecretAsync
            (
                // vault path within kv-v2/ (e.g. "api-key", not "kv-v2/api-key")
                path: _settings.ApiKeyPath
            ).Result;

            string apiKey = secret.Data.Data[ _settings.ApiKeyField ].ToString();

            _logger.LogInformation("getting secret api key from vault: done");

            return apiKey;
        }

        public UsernamePasswordCredentials GetDatabaseCredentials()
        {
            _logger.LogInformation("getting temporary database credentials from vault: started");

            Secret<UsernamePasswordCredentials> dynamicDatabaseCredentials = _client.V1.Secrets.Database.GetCredentialsAsync
            (
                // vault path within database/roles/ (e.g. "dev-readonly", not "database/roles/dev-readonly")
                roleName: _settings.DatabaseCredentialsRole
            ).Result;

            _logger.LogInformation("getting temporary database credentials from vault: done");

            return dynamicDatabaseCredentials.Data;
        }
    }
}
