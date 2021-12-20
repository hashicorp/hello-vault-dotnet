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
    public class VaultSettings
    {
        public string Address { get; set; }

        public string AppRoleRoleId { get; set; }
        public string AppRoleSecretIdPath { get; set; }
    }
}