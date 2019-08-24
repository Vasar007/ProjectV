using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using ThingAppraiser.ConfigurationWebService.v1.Domain;

namespace ThingAppraiser.ConfigurationWebService
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfigCreator, ConfigCreator>();

            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApiVersioning(
                options =>
                {
                    // Reporting api versions will return the headers "api-supported-versions" and 
                    // "api-deprecated-versions".
                    options.ReportApiVersions = true;

                    // Automatically applies an api version based on the name of the defining 
                    // controller's namespace.
                    options.Conventions.Add(new VersionByNamespaceConvention());
                }
            );

            // Register the Swagger generator, defining 1 or more Swagger documents.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "ThingAppraiser Configuration API",
                    Description = "Web API to create service configurations based on input " +
                                  "parameters.",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Vasily Vasilyev",
                        Email = "vasar007@yandex.ru",
                        Url = "https://t.me/Vasar007"
                    },
                    License = new License
                    {
                        Name = "Apache License 2.0",
                        Url = "http://www.apache.org/licenses/LICENSE-2.0"
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request 
        // pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production 
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json",
                                  "ThingAppraiser Configuration API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
