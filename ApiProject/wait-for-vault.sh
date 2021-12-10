#!/bin/sh
echo "Waiting for vault to start so we can obtain a RoleID"
sleep 30;

while [ ! -f "${1}" ]; do sleep 3; done

sleep 2;
echo "Starting ${2}"

export VAULT_APPROLE_ROLE_ID=$(cat /tmp/role)

dotnet "WebApi.dll"