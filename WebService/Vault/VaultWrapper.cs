using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines;

namespace WebService.Vault
{
    public class VaultWrapper
    {
        private IVaultClient _client;
        private readonly VaultWrapperSettings _settings;

        public VaultWrapper( VaultWrapperSettings settings )
        {
            _client = AppRoleAuthClient( settings );
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
            // The wrapping token is placed here by our trusted orchestrator
            string wrappingToken = File.ReadAllText(settings.AppRoleAuthSecretIdFile).Trim();

            // We can't reuse the default VaultClient instance for unwrapping because
            // it needs to be intialized with a different TokenAuthMethodInfo
            IVaultClient vaultClientForUnwrapping = new VaultClient(new VaultClientSettings(
                settings.Address,
                new TokenAuthMethodInfo(vaultToken: wrappingToken)
            ));

            // We pass null here instead of the wrapping token to avoid depleting
            // the token's single use. This is to work around the fact that VaultSharp
            // requires a valid wrapping token to initialize the VaultClient.
            string appRoleAuthSecretId
                = vaultClientForUnwrapping.V1.System
                    .UnwrapWrappedResponseDataAsync<Dictionary<string, object>>(tokenId: null)
                        .Result.Data[ "secret_id" ]
                            .ToString();

            AppRoleAuthMethodInfo appRoleAuth = new AppRoleAuthMethodInfo(
                roleId: settings.AppRoleAuthRoleId,
                secretId: appRoleAuthSecretId
            );

            IVaultClient client = new VaultClient( new VaultClientSettings(
                settings.Address,
                appRoleAuth
            ));

            return client;
        }

        public string GetSecretApiKey()
        {
            Secret<SecretData> secret = _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: _settings.ApiKeyPath // vault path within kv-v2/ (e.g. "api-key", not "kv-v2/api-key" )
            ).Result;

            string apiKey = secret.Data.Data[ _settings.ApiKeyField ].ToString();
            return apiKey;
        }

        public string GetDbConnectionString()
        {
            Secret<UsernamePasswordCredentials> dynamicDatabaseCredentials =
                _client.V1.Secrets.Database.GetCredentialsAsync(
                _settings.DatabaseCredentialsRole).Result;

            string userId = dynamicDatabaseCredentials.Data.Username;
            string password = dynamicDatabaseCredentials.Data.Password;
            
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            // To Do: Get from appsettings or env var
            builder.DataSource = "database";
            builder.InitialCatalog = "example";

            builder.UserID = userId;
            builder.Password = password;

            return builder.ConnectionString;
        }
    }
}
