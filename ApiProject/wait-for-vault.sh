#!/bin/sh
echo "Waiting for orchestrator"
sleep 15; 

while [ ! -f /tmp/secret ]; do sleep 3; done

sleep 2;

dotnet "WebApi.dll"