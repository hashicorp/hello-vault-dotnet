# hello-vault-dotnet

This is a sample application that demonstrates how to authenticate to and
retrieve secrets from HashiCorp [Vault][vault] using the
[VaultSharp][vaultsharp] client library.

## Prerequisites

1. [`docker`][docker] to easily run the application in the same environment
   regardless of your local operating system
1. [`docker compose`][docker-compose] to easily set up all the components of the
   demo (the application's web server, the Vault server, the database, etc.) all
   at once
1. [`curl`][curl] to test our endpoints
1. [`jq`][jq] (optional) for prettier `JSON` output

## Try it out

> **WARNING**: The Vault server used in this setup is configured to run in
> `-dev` mode, an insecure setting that allows for easy testing.

### 1. Bring up the services

This step may take a few minutes to download the necessary dependencies.

```shell-session
./run.sh
```

The script will bring up the web app and all of its dependencies using an
architecture-specific `docker-compose.*.yaml` file:

```
Restarting with docker-compose.arm64.yaml
...
[+] Running 8/8
 ⠿ Network hello-vault-dotnet_default                       Created       0.0s
 ⠿ Volume "hello-vault-dotnet_trusted-orchestrator-volume"  Created       0.0s
 ⠿ Container hello-vault-dotnet-database-1                  Started       0.5s
 ⠿ Container hello-vault-dotnet-secure-service-1            Started       0.6s
 ⠿ Container hello-vault-dotnet-database-data-1             Started       1.3s
 ⠿ Container hello-vault-dotnet-vault-server-1              Started       8.0s
 ⠿ Container hello-vault-dotnet-trusted-orchestrator-1      Started      14.8s
 ⠿ Container hello-vault-dotnet-app-1                       Started      16.6s
```

Verify that the services started successfully:

```shell-session
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

```
hello-vault-dotnet-app-1                    Up About a minute (healthy)   0.0.0.0:80->80/tcp
hello-vault-dotnet-trusted-orchestrator-1   Up About a minute (healthy)
hello-vault-dotnet-vault-server-1           Up About a minute (healthy)   0.0.0.0:8200->8200/tcp
hello-vault-dotnet-secure-service-1         Up About a minute (healthy)   0.0.0.0:8080->80/tcp
hello-vault-dotnet-database-1               Up About a minute             1401/tcp, 0.0.0.0:1433->1433/tcp
```

### 2. Try out `POST /api/Payments` endpoint (static secrets workflow)

`POST /api/Payments` endpoint is a simple example of the static secrets
workflow. Our service will make a request to another service's restricted API
endpoint using an API key value stored in Vault's static secrets engine.

```shell-session
curl -s -X POST --header "Content-Length: 0" http://localhost/api/Payments | jq
```

```json
{
  "message": "hello world!"
}
```

Check the logs:

```shell-session
docker logs hello-vault-dotnet-app-1 2>&1 | grep "api key"
```

```log
retrieving api key from Vault: started
getting secret api key from vault: started
getting secret api key from vault: done
retrieving api key from Vault: done
sent request to http://secure-service/api with api key and received a response
```

### 3. Try out `GET /api/Products` endpoint (dynamic secrets workflow)

`GET /api/Products` endpoint is a simple example of the dynamic secrets
workflow. Our application uses Vault's database secrets engine to generate
dynamic database credentials in our MSSQL database. The credentials are
then used by to connect to and retrieve data from the database.

```shell-session
curl -s -X GET --header "Content-Length: 0" http://localhost/api/Products | jq
```

```json
[
  {
    "id": 1,
    "name": "Rustic Webcam"
  },
  {
    "id": 2,
    "name": "Haunted Coloring Book"
  }
]
```

Check the logs:

```shell-session
docker logs hello-vault-dotnet-app-1 | grep "database"
```

```log
getting temporary database credentials from vault: started
getting temporary database credentials from vault: done
connecting to 'tcp:database,1433' database with username v-approle-dev-readonly-1pdIiChXK790ErrolCIa-1641254312: started
connecting to 'tcp:database,1433' database with username v-approle-dev-readonly-1pdIiChXK790ErrolCIa-1641254312: done
fetching products from database: started
fetching products from database: done
```

[vault]:           https://www.vaultproject.io/
[vaultsharp]:      https://github.com/rajanadar/VaultSharp
[docker]:          https://docs.docker.com/get-docker/
[docker-compose]:  https://docs.docker.com/compose/install/
[curl]:            https://curl.se/
[jq]:              https://stedolan.github.io/jq/
