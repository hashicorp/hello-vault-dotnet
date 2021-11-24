using System;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public static class VaultExtension
    {
        public static IConfigurationBuilder AddVault(this IConfigurationBuilder configuration, Action<VaultSettings> settings)
        {
            var vaultOptions = new VaultConfigurationSource(settings);
            configuration.Add(vaultOptions);
            return configuration;
        }
    }
}