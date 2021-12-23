namespace WebService.Vault
{
    public class VaultWrapperSettings
    {
        // Vault server address
        public string Address { get; set; }

        // AppRole credentials used to authenticate with Vault
        public string AppRoleAuthRoleId { get; set; }
        public string AppRoleAuthSecretIdFile { get; set; }

        // Location and field name of the api key secret within Vault
        public string ApiKeyPath { get; set; }
        public string ApiKeyField { get; set; }

        /// Dynamic secrets variables
        public string DatabaseCredentialsRole { get; set; }
    }
}
