using System;

namespace WebApi
{
    public class VaultSettings
    {
        public string Address { get; set; }
        public string RoleName { get; set; }
        public string MountPath { get; set; }
        public string TokenPath { get; set; }
    }
}