#!/bin/sh

set -eux pipefail

## WARNING: It is insecure to configure Vault inside an application's entrypoint script like this.
## An operator should be performing these steps separately. 
## It is only done like this here to simplify the out-of-the-box hello-world experience for the user.

export VAULT_ADDR="http://127.0.0.1:8200"
export VAULT_TOKEN="root" # WARNING: insecure

mkdir -p /app/path/to

curl -X PUT -H "X-Vault-Token: ${VAULT_TOKEN}" -H "X-Vault-Wrap-Ttl: 5m10s" \
    -d "null" ${VAULT_ADDR}/v1/auth/approle/role/dev-role/secret-id | jq -r .wrap_info.token > /app/path/to/wrapping-token

dotnet "WebApi.dll"