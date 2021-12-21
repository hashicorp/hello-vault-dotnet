using System.Collections.Generic;
using System.IO;
using VaultSharp;
using VaultSharp.V1.AuthMethods.AppRole;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace app.Vault
{
    public class VaultWrapper
    {
        protected IVaultClient _client;
        protected readonly VaultWrapperSettings _settings;

        public VaultWrapper( VaultWrapperSettings settings )
        {
            _settings = settings;

            _client = AppRoleAuthClient( settings );
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
        private IVaultClient AppRoleAuthClient( VaultWrapperSettings settings )
        {
            // The wrapping token is placed here by our trusted orchestrator
            string wrappingToken = File.ReadAllText( settings.AppRoleAuthSecretIdFile ).Trim();

            // We can't reuse the default VaultClient instance for unwrapping because
            // it needs to be intialized with a different TokenAuthMethodInfo
            IVaultClient vaultClientForUnwrapping = new VaultClient(
                new VaultClientSettings(
                    vaultServerUriWithPort: settings.Address,
                    authMethodInfo: new TokenAuthMethodInfo( vaultToken: wrappingToken )
                )
            );

            // We pass null here instead of the wrapping token to avoid depleting
            // the token's single use. This is to work around the fact that VaultSharp
            // requires a valid wrapping token to initialize the VaultClient.
            string appRoleAuthSecretId
                = vaultClientForUnwrapping.V1.System
                    .UnwrapWrappedResponseDataAsync<Dictionary<string, object>>( tokenId: null )
                        .Result.Data[ "secret_id" ]
                            .ToString();

            AppRoleAuthMethodInfo appRoleAuth = new AppRoleAuthMethodInfo(
                roleId: settings.AppRoleAuthRoleId,
                secretId: appRoleAuthSecretId
            );

            return new VaultClient(
                new VaultClientSettings(
                    vaultServerUriWithPort: settings.Address,
                    authMethodInfo: appRoleAuth
                )
            );
        }

        public string GetSecretApiKey()
        {
            Secret<SecretData> secret = _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: _settings.ApiKeyPath // vault path within kv-v2/ (e.g. "api-key", not "kv-v2/api-key" )
            ).Result;

            return secret.Data.Data[_settings.ApiKeyDescriptor].ToString();
        }
    }
}