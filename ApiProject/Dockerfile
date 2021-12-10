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

COPY wait-for-vault.sh /app
COPY --from=build-env /app/out .
RUN chmod +x /app/wait-for-vault.sh
