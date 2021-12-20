namespace app.Vault
{
    public class VaultWrapperSettings
    {
        public string Address { get; set; }

        public string AppRoleAuthRoleId { get; set; }
        public string AppRoleAuthSecretIdFile { get; set; }

        public string ApiKeyPath { get; set; }
        public string ApiKeyDescriptor { get; set; }
    }
}
