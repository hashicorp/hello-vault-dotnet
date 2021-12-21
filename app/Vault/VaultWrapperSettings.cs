namespace app.Vault
{
    public class VaultWrapperSettings
    {
        public string Address { get; set; }

        /// Approle Auth 
        public string AppRoleAuthRoleId { get; set; }
        public string AppRoleAuthSecretIdFile { get; set; }

        /// Static Secrets 
        public string ApiKeyPath { get; set; }
        public string ApiKeyDescriptor { get; set; }

        /// Dynamic Secrets

        public string DynamicSecretRole { get; set; }
    }
}
