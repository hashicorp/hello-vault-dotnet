using System;

namespace WebApi
{
    public class VaultSettings
    {
        public string Address { get; set; }
        public string RoleIdPath { get; set; }
        public string MountPath { get; set; }
        public string TokenPath { get; set; }
    }
}