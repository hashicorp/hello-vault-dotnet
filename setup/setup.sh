#!/bin/bash

set -euox pipefail

export VAULT_ADDR="http://0.0.0.0:8200"
export VAULT_TOKEN="root"

docker-compose down
docker-compose up -d 

sleep 1

vault policy write dev-policy dev-policy.hcl

vault auth enable approle

vault write auth/approle/role/dev-role token_policies=dev-policy