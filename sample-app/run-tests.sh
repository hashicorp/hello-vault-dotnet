#!/bin/sh
# Copyright (c) HashiCorp, Inc.
# SPDX-License-Identifier: MPL-2.0


################################ TEST PLAN ################################
# 1. bring up docker-compose environment (using architecture-specific file)
# 2. curl our endpoints & verify the results
# 3. bring down the docker-compose environment

WEB_SERVICE_ADDRESS="http://localhost"

# pick architecture-specific docker-compose file
if [ "$(uname -m)" = "arm64" ]
then
    DOCKER_COMPOSE_FILE="docker-compose.arm64.yaml"
else
    DOCKER_COMPOSE_FILE="docker-compose.yaml"
fi

echo "Running tests using ${DOCKER_COMPOSE_FILE}"

# bring up hello-vault-go service and its dependencies
docker compose -f "${DOCKER_COMPOSE_FILE}" up -d --build --quiet-pull

# bring down the services on exit
trap 'docker compose -f ${DOCKER_COMPOSE_FILE} down --volumes' EXIT

# TEST 1: POST /api/Payments (static secrets)
output1=$(curl --silent --request POST --header "Content-Length: 0" "${WEB_SERVICE_ADDRESS}/api/Payments")

echo "[TEST 1]: output: $output1"

if [ "${output1}" != '{"message":"hello world!"}' ]
then
    echo "[TEST 1]: FAILED: unexpected output"
    exit 1
else
    echo "[TEST 1]: OK"
fi

# TEST 2: GET /api/Products (dynamic secrets)
output2=$(curl --silent --request GET --header "Content-Length: 0" "${WEB_SERVICE_ADDRESS}/api/Products")

echo "[TEST 2]: output: $output2"

if [ "${output2}" != '[{"id":1,"name":"Rustic Webcam"},{"id":2,"name":"Haunted Coloring Book"}]' ]
then
    echo "[TEST 2]: FAILED: unexpected output"
    exit 1
else
    echo "[TEST 2]: OK"
fi
