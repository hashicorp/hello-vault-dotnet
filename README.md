# hello-vault-dotnet
This is a sample application that demonstrates how to authenticate to and retrieve secrets from HashiCorp [Vault][vault] using the [VaultSharp][vaultsharp] client library.

## Prerequisites

1. [`docker`][docker] to easily run the application in the same environment regardless of your local operating system
2. [`docker compose`][docker-compose] to easily set up all the components of the demo (the application's web server, the Vault server, the database, etc.) all at once
3. [`curl`] to test our endpoints
4. [`jq`] (optional) for prettier `JSON` output

## Try it out

> **WARNING**: The Vault server used in this setup is configured to run in
> `-dev` mode, an insecure setting that allows for easy testing.

### 1. Bring up the services

This step may take a few minutes to download the necessary dependencies.

```bash
./run.sh
```

```
[+] Running 7/7
 ⠿ Network hello-vault-go_default                          Created        0.1s
 ⠿ Volume "hello-vault-go_trusted-orchestrator-volume"     Created        0.0s
 ⠿ Container hello-vault-go-secure-service-1               Started        0.6s
 ⠿ Container hello-vault-go-database-1                     Started        0.6s
 ⠿ Container hello-vault-go-vault-server-1                 Started        1.3s
 ⠿ Container hello-vault-go-trusted-orchestrator-1         Started        8.6s
 ⠿ Container hello-vault-go-app-1                          Started       10.3s

```

Verify that the services started successfully:

```bash
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
```

```
NAMES                                   STATUS                        PORTS
hello-vault-go-app-1                    Up About a minute (healthy)   0.0.0.0:8080->8080/tcp
hello-vault-go-trusted-orchestrator-1   Up About a minute (healthy)
hello-vault-go-vault-server-1           Up About a minute (healthy)   0.0.0.0:8200->8200/tcp
hello-vault-go-secure-service-1         Up About a minute (healthy)   0.0.0.0:1717->80/tcp
hello-vault-go-database-1               Up About a minute (healthy)   0.0.0.0:5432->5432/tcp
```

[vault]:           https://www.vaultproject.io/
[vaultsharp]:      https://github.com/rajanadar/VaultSharp
[docker]:          https://docs.docker.com/get-docker/
[docker-compose]:  https://docs.docker.com/compose/install/
[curl]:            https://curl.se/
[jq]:              https://stedolan.github.io/jq/
