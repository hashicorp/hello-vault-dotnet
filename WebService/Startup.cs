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

            // TODO: initialize the parameters from environment variables instead
            services.AddSingleton< VaultWrapper >( new VaultWrapper( new VaultWrapperSettings{
                Address                 = "http://vault-server:8200",
                AppRoleAuthRoleId       = "demo-web-app",
                AppRoleAuthSecretIdFile = "/tmp/secret",
                ApiKeyPath              = "api-key",
                ApiKeyField             = "api-key-descriptor"
            }));
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
