using System;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class VaultConfigurationSource : IConfigurationSource
    {
        private app.Vault.VaultSettings _settings;

        public VaultConfigurationSource(Action<VaultSettings> vaultSettings)
        {
            _settings = new VaultSettings();
            vaultSettings.Invoke(_settings);
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new VaultConfigurationProvider(_settings);
    }
}
