# This section grants all access on "kv-v2/data/api-key*"
path "kv-v2/data/api-key/*" {
  capabilities = ["read", "update"]
}

# Even though we allowed secret/*, this line explicitly denies
# secret/super-secret. This takes precedence.
path "secret/super-secret" {
  capabilities = ["deny"]
}