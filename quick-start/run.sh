#!/bin/sh
# Copyright (c) HashiCorp, Inc.
# SPDX-License-Identifier: MPL-2.0


echo "Starting Vault dev server.."
container_id=$(docker run --rm --detach -p 8200:8200 -e 'VAULT_DEV_ROOT_TOKEN_ID=dev-only-token' vault)

echo "Running quickstart example."
dotnet run Program.cs

echo "Stopping Vault dev server.."
docker stop "${container_id}" > /dev/null

echo "Vault server has stopped."