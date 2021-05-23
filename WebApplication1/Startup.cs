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
    public class CustomModelDocumentFilter<T> : IDocumentFilter where T : class
    {
        public void Apply ( OpenApiDocument openapiDoc, DocumentFilterContext context )
        {            
            context.SchemaGenerator.GenerateSchema( typeof( T ), context.SchemaRepository );
        }
    }


    public class ResponseWrapperOperationFilter : IOperationFilter
    {
        public void Apply ( OpenApiOperation operation, OperationFilterContext context )
        {
            var responseType = context.ApiDescription.SupportedResponseTypes[0].Type;
            var wrappedResponseType = typeof( ResponseWrapper<> ).MakeGenericType( responseType );

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
        }
    }

            //Schema = new OpenApiSchema()
            //{
            //    Reference = new OpenApiReference
            //    {
            //        Type = ReferenceType.Schema,
            //        Id = wrappedReturnType.Name
            //    }
            //}
            //(new System.Collections.Generic.ICollectionDebugView<Microsoft.AspNetCore.Mvc.ApiExplorer.ApiResponseType>( context.ApiDescription.SupportedResponseTypes ).Items[0]).Type


        
    
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

                 //c.DocumentFilter<CustomModelDocumentFilter<ResponseWrapper2>>();

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
