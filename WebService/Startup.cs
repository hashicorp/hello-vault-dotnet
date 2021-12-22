using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WebService.Vault;

namespace WebService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<VaultWrapper>( new VaultWrapper( new VaultWrapperSettings{
                Address                 = Environment.GetEnvironmentVariable( "VAULT_ADDRESS" ),
                AppRoleAuthRoleId       = Environment.GetEnvironmentVariable( "VAULT_APPROLE_ROLE_ID" ),
                AppRoleAuthSecretIdFile = Environment.GetEnvironmentVariable( "VAULT_APPROLE_SECRET_ID_FILE" ),
                ApiKeyPath              = Environment.GetEnvironmentVariable( "VAULT_API_KEY_PATH" ),
                ApiKeyField             = Environment.GetEnvironmentVariable( "VAULT_API_KEY_FIELD" ),
            }));

            services.AddSingleton<string>( Environment.GetEnvironmentVariable( "SECURE_SERVICE_ADDRESS" ) );
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
    }
}
