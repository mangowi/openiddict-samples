using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace Zirku.Api2
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

            // Register the OpenIddict validation components.
            services.AddOpenIddict()
                .AddValidation(options =>
                {
                    // Note: the validation handler uses OpenID Connect discovery
                    // to retrieve the address of the introspection endpoint.
                    options.SetIssuer("http://localhost:12345/");
                    options.AddAudiences("resource_server_2");

                    // Configure the validation handler to use introspection and register the client
                    // credentials used when communicating with the remote introspection endpoint.
                    options.UseIntrospection()
                           .SetClientId("resource_server_2")
                           .SetClientSecret("C744604A-CD05-4092-9CF8-ECB7DC3499A2");

                    // Register the System.Net.Http integration.
                    options.UseSystemNetHttp();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:9000");
                builder.WithMethods("GET");
                builder.WithHeaders("Authorization");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(options =>
            {
                options.MapControllerRoute(
                    name: "empty",
                    pattern: "{controller=Resource}/{action=Public}"
                );

                options.MapControllerRoute(
                    name: "api",
                    pattern: "api/{action=Public}",
                    defaults: new { controller = "Resource" }
                );
            });
        }
    }
}
