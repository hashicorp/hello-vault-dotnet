#!/bin/bash

set -euox pipefail

export VAULT_ADDR="http://0.0.0.0:8200"
export VAULT_TOKEN="root"

docker-compose down
docker-compose up -d

sleep 1

# Enable the auth method AppRole
vault auth enable approle

# Create a policy for secrets access
vault policy write dev-policy dev-policy.hcl

vault write auth/approle/role/dev-role token_policies=dev-policy

# Enable key-value secrets backend
vault secrets enable kv-v2

# Add our api key to our key-value store
vault kv put secret/api-key key="myapikey"