using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VaultSharp.V1.SecretsEngines;
using WebService.Controllers;
using WebService.Database;
using WebService.Vault;

namespace WebService
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;

        public Startup(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            VaultWrapper vault = new VaultWrapper
            (
                _loggerFactory,
                new VaultWrapperSettings
                {
                    Address                 = GetEnvironmentVariableOrThrow("VAULT_ADDRESS"),
                    AppRoleAuthRoleId       = GetEnvironmentVariableOrThrow("VAULT_APPROLE_ROLE_ID"),
                    AppRoleAuthSecretIdFile = GetEnvironmentVariableOrThrow("VAULT_APPROLE_SECRET_ID_FILE"),
                    ApiKeyPath              = GetEnvironmentVariableOrThrow("VAULT_API_KEY_PATH"),
                    ApiKeyField             = GetEnvironmentVariableOrThrow("VAULT_API_KEY_FIELD"),
                    DatabaseCredentialsRole = GetEnvironmentVariableOrThrow("VAULT_DATABASE_CREDENTIALS_ROLE")
                }
            );

            UsernamePasswordCredentials credentials = vault.GetDatabaseCredentials();

            DatabaseClient database = new DatabaseClient
            (
                _loggerFactory,
                new DatabaseSettings
                {
                    DataSource        = GetEnvironmentVariableOrThrow("DATABASE_DATA_SOURCE"),
                    InitialCatalog    = GetEnvironmentVariableOrThrow("DATABASE_INITIAL_CATALOG"),
                    Timeout = int.Parse(GetEnvironmentVariableOrThrow("DATABASE_TIMEOUT"))
                },
                credentials.Username,
                credentials.Password
            );

            var paymentsControllerSettings = new PaymentsControllerSettings
            {
                SecureServiceEndpoint = GetEnvironmentVariableOrThrow("SECURE_SERVICE_ENDPOINT")
            };

            // Inject dependencies as singletons
            services.AddSingleton<VaultWrapper>(vault);
            services.AddSingleton<DatabaseClient>(database);
            services.AddSingleton<PaymentsControllerSettings>(paymentsControllerSettings);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static string GetEnvironmentVariableOrThrow(string variable)
        {
            var value = Environment.GetEnvironmentVariable(variable);

            if (String.IsNullOrEmpty(value))
            {
                throw new ApplicationException($"the required environment variable '{ variable }' is not set");
            }

            return value;
        }
    }
}
