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

namespace app.Vault
{
    public class Vault
    {
        protected IVaultClient _client;

        public Vault( VaultSettings settings )
        {
            _client = AppRoleAuthClient( settings );
        }

        /// <summary>
        /// A combination of a RoleID and a SecretID is required to log into Vault
        /// with AppRole authentication method. The SecretID is a value that needs
        /// to be protected, so instead of the app having knowledge of the SecretID
        /// directly, we have a trusted orchestrator (simulated with a script here)
        /// give the app access to a short-lived response-wrapping token.
        ///
        /// ref: https://www.vaultproject.io/docs/concepts/response-wrapping
        /// ref: https://learn.hashicorp.com/tutorials/vault/secure-introduction?in=vault/app-integration#trusted-orchestrator
        /// ref: https://learn.hashicorp.com/tutorials/vault/approle-best-practices?in=vault/auth-methods#secretid-delivery-best-practices
        /// </summary>
        private IVaultClient AppRoleAuthClient( VaultSettings settings )
        {
            // The wrapping token is placed here by our trusted orchestrator
            string wrappingToken = File.ReadAllText( settings.AppRoleAuthSecretIdFile ).Trim();

            // We can't reuse the default VaultClient instance for unwrapping because
            // it needs to be intialized with a different TokenAuthMethodInfo
            IVaultClient vaultClientForUnwrapping = new VaultClient(
                new VaultClientSettings(
                    settings.Address,
                    new TokenAuthMethodInfo( wrappingToken )
                )
            );

            // We pass null here instead of the wrapping token to avoid depleting
            // the token's single use. This is to work around the fact that VaultSharp
            // requires a valid wrapping token to initialize the VaultClient.
            string appRoleAuthSecretId
                = vaultClientForUnwrapping.V1.System
                    .UnwrapWrappedResponseDataAsync< Dictionary< string, object > >( null )
                        .Result.Data[ "secret_id" ]
                            .ToString();

            AppRoleAuthMethodInfo authMethodInfo = new AppRoleAuthMethodInfo(
                settings.AppRoleAuthRoleId,
                appRoleAuthSecretId
            );

            return new VaultClient( new VaultClientSettings( settings.Address, authMethodInfo ) );
        }
    }



}