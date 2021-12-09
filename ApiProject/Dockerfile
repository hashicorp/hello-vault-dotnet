FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj 
COPY *.csproj /app
RUN dotnet restore

# Copy all files
COPY . /app
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
		curl \
        jq

COPY entrypoint.sh /app/entrypoint.sh
COPY --from=build-env /app/out .
ENTRYPOINT [ "/app/entrypoint.sh" ]
