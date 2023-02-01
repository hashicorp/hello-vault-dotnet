#!/bin/sh
# Copyright (c) HashiCorp, Inc.
# SPDX-License-Identifier: MPL-2.0


###############################################################################################
##               *** WARNING - INSECURE - DO NOT USE IN PRODUCTION ***                       ##
## This script is to simulate operations a Vault operator would perform and, as such,        ##
## is not a representation of best practices in production environments.                     ##
## https://learn.hashicorp.com/tutorials/vault/pattern-approle?in=vault/recommended-patterns ##
###############################################################################################

set -e

export VAULT_ADDR='http://127.0.0.1:8200'
export VAULT_FORMAT='json'

# Spawn a new process for the development Vault server and wait for it to come online
# ref: https://www.vaultproject.io/docs/concepts/dev-server
vault server -dev -dev-listen-address="0.0.0.0:8200" &
sleep 5s

# Authenticate container's local Vault cli
# ref: https://www.vaultproject.io/docs/commands/login
vault login -no-print "${VAULT_DEV_ROOT_TOKEN_ID}"

#####################################
########## ACCESS POLICIES ##########
#####################################

# Add policies for the various roles we'll be using
# ref: https://www.vaultproject.io/docs/concepts/policies
vault policy write trusted-orchestrator-policy /vault/config/trusted-orchestrator-policy.hcl
vault policy write dev-policy /vault/config/dev-policy.hcl

#####################################
######## APPROLE AUTH METHDO ########
#####################################

# Enable AppRole auth method utilized by our web application
# ref: https://www.vaultproject.io/docs/auth/approle
vault auth enable approle

# Configure a specific AppRole role with associated parameters. The tokens
# are not currently being renewed by the application, so we default to long
# ttl durations.
# ref: https://www.vaultproject.io/api/auth/approle#parameters
vault write auth/approle/role/dev-role \
    token_policies=dev-policy \
    secret_id_ttl="48h" \
    token_ttl="48h" \
    token_max_ttl="768h"

# Overwrite our role id with a known value to simplify our demo
vault write auth/approle/role/dev-role/role-id role_id="${APPROLE_ROLE_ID}"

#####################################
######### TOKEN AUTH METHOD #########
#####################################

# Configure a token with permissions to act as a trusted orchestrator.
# For simplicity, we don't handle renewals in our simulated orchestrator
# so we've set the ttl to a very long duration (768h). When this expires
# the web app will no longer receive a secret id and subsequently fail on the
# next attempted AppRole login.
# ref: https://www.vaultproject.io/docs/commands/token/create
vault token create \
    -id="${ORCHESTRATOR_TOKEN}" \
    -policy=trusted-orchestrator-policy \
    -ttl="768h"

#####################################
########## STATIC SECRETS ###########
#####################################

# Enable a kv-v2 secrets engine, passing in the path parameter
# ref: https://www.vaultproject.io/docs/secrets/kv/kv-v2
vault secrets enable -path=kv-v2 kv-v2

# Seed the kv-v2 store with an entry our web app will use
vault kv put kv-v2/api-key api-key-descriptor=my-secret-key

#####################################
########## DYNAMIC SECRETS ##########
#####################################

# Enable a database secrets engine
# ref: https://www.vaultproject.io/docs/secrets/databases
vault secrets enable database

# Configure Vault's connection to our db, in this case PostgreSQL
# ref: https://www.vaultproject.io/docs/secrets/databases/mssql
vault write database/config/example \
    plugin_name=mssql-database-plugin \
    allowed_roles="dev-readonly" \
    connection_url='sqlserver://{{username}}:{{password}}@database:1433' \
    username="vault-db-user" \
    password="VaultDatabasePassword1"

# Allow Vault to create roles dynamically with the same privileges as the
# 'readonly' role created in our database's init scripts. The tokens
# are not currently being renewed by the application, so we default to long
# ttl durations.
vault write database/roles/dev-readonly \
    db_name=example \
    creation_statements="CREATE LOGIN [{{name}}] WITH PASSWORD = '{{password}}';\
        CREATE USER [{{name}}] FOR LOGIN [{{name}}]; \
        ALTER ROLE [vault_datareader] ADD MEMBER [{{name}}];" \
    default_ttl="48h" \
    max_ttl="768h"

# This container is now healthy
touch /tmp/healthy

# Keep container alive
tail -f /dev/null & trap 'kill %1' TERM ; wait
