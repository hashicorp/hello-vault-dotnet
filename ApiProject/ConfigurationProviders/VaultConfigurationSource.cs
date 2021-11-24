using System;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    public class VaultConfigurationSource : IConfigurationSource
    {
        private VaultSettings _vaultSettings;

        public VaultConfigurationSource(Action<VaultSettings> vaultSettings)
        {
            _vaultSettings = new VaultSettings();
            vaultSettings.Invoke(_vaultSettings);
        }
        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new VaultConfigurationProvider(_vaultSettings);
    }
}