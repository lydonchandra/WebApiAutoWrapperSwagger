using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoWrapper;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class AutoWrapperDefinition<T>
    {
        public string Message { get; set; }
        public T Result { get; set; }
    }

    public class ResponseWrapperOperationFilter : IOperationFilter
    {
        public void Apply ( OpenApiOperation operation, OperationFilterContext context )
        {
            //eg. IEnumerable<WeatherForecast>
            var responseType = context.ApiDescription.SupportedResponseTypes[0].Type;

            //eg. AutoWrapper<IEnumerable<WeatherForecast>>
            var wrappedResponseType = typeof( AutoWrapperDefinition<> ).MakeGenericType( responseType );

            //new schema, TODO is to check if schema exists
            var schema = context.SchemaGenerator.GenerateSchema(
                wrappedResponseType,
                context.SchemaRepository
                );

            var openApiResponse = new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>()
                {
                    ["application/json"] = new OpenApiMediaType()
                    {
                        Schema = schema
                    }
                },
            };
            operation.Responses.Clear();
            operation.Responses.Add( "200", openApiResponse );
            //TODO: add Response for other status code
        }
    }
        
    
    public class Startup
    {
        public Startup ( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices ( IServiceCollection services )
        {

            services.AddControllers();
            services.AddSwaggerGen( c =>
             {
                 c.OperationFilter<ResponseWrapperOperationFilter>();
                 c.SwaggerDoc( "v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" } );
             } );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure ( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI( c => c.SwaggerEndpoint( "/swagger/v1/swagger.json", "WebApplication1 v1" ) );
            }

            app.UseHttpsRedirection();

            app.UseApiResponseAndExceptionWrapper();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints( endpoints =>
             {
                 endpoints.MapControllers();
             } );
        }
    }
}
