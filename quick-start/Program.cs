// Copyright (c) HashiCorp, Inc.
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace DeveloperQuickstart
{
    class Program
    {
        static void Main(string[] args)
        {
            // Authenticate
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: "dev-only-token");

            VaultClientSettings vaultClientSettings = new VaultClientSettings("http://127.0.0.1:8200", authMethod);
            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            // Write a secret
            var secretData = new Dictionary<string, object> { { "password", "Hashi123" } };
            vaultClient.V1.Secrets.KeyValue.V2.WriteSecretAsync(
                path: "/my-secret-password",
                data: secretData,
                mountPoint: "secret"
            ).Wait();

            Console.WriteLine("Secret written successfully.");

            // Read a secret
            Secret<SecretData> secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: "/my-secret-password",
                mountPoint: "secret"
            ).Result;

            var password = secret.Data.Data["password"];

            if (password.ToString() != "Hashi123")
            {
                throw new System.Exception("Unexpected password");
            }

            Console.WriteLine("Access granted!");
        }
    }
}
