using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;

namespace WebApplication1
{
    public class Startup
    {
        public const string DatabaseName = "ResourceTypes";
        public const string ContainerName = "accounts";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string accountUrl = "";
            string accountSecret = "";

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var builder = ErrorResponse.Builder()
                           .WithMessage("One or more errors")
                           .WithErrorCode("InvalidParameter")
                           .WithTarget("property");

                        foreach (var entry in context.ModelState.Where(entry => entry.Value.ValidationState == ModelValidationState.Invalid))
                        {
                            foreach (var error in entry.Value.Errors)
                            {
                                builder.AddErrorDetail("InvalidParameter", error.ErrorMessage, entry.Key ?? "property");
                            }
                        }

                        return new ContentResult()
                        {
                            Content = JsonConvert.SerializeObject(builder.Build()),
                            ContentType = MediaTypeConstants.JsonMediaType,
                            StatusCode = (int)HttpStatusCode.BadRequest
                        };
                    };
                });
            services.AddOData();


            services.AddDbContext<AccountsContext>(options =>
            {
                options.UseCosmos(
                    accountUrl,
                    accountSecret,
                    DatabaseName,
                    options =>
                        options
                        .ConnectionMode(ConnectionMode.Direct));
            });

            services.AddSingleton((sp) =>
            {
                var client = new CosmosClient(accountUrl, accountSecret, new CosmosClientOptions()
                {
                    ConnectionMode = ConnectionMode.Direct,
                    EnableTcpConnectionEndpointRediscovery = true,
                    SerializerOptions = new CosmosSerializationOptions()
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                });
                return client;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseErrorHandling();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().OrderBy().Count().MaxTop(10).SkipToken();
                endpoints.MapODataRoute("poco", "odata", container =>
                {
                    container.AddService(Microsoft.OData.ServiceLifetime.Singleton, sp => GetEdmModel());
                });
            });
        }

        private static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EnableLowerCamelCase();
            odataBuilder.EntitySet<PocoSample>("poco");
            var model = odataBuilder.GetEdmModel();
            return model;
        }
    }
}
