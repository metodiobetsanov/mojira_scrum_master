//  <copyright file="CustomSwaggerExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Extensions
{
    using System;

    using Filters;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;

    public static class CustomSwaggerExtension
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, string appName)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                                         {
                                             Version = "v1",
                                             Title = "Mojira SM Identity API",
                                             Description = $"{appName} API for Mojira Scrum Master",
                                             Contact = new OpenApiContact
                                                       {
                                                           Name = "MDO",
                                                           Email = "htc.sens1@gmail.com",
                                                           Url = new Uri("https://github.com/metodiobetsanov")
                                                       },
                                             License = new OpenApiLicense
                                                       {
                                                           Name = "Use under MIT",
                                                           Url = new Uri(
                                                               "https://github.com/metodiobetsanov/mojira_scrum_master/blob/main/LICENSE")
                                                       }
                                         });

                options.AddSecurityDefinition("Bearer",
                                              new OpenApiSecurityScheme
                                              {
                                                  In = ParameterLocation.Header,
                                                  Name = "Authorization",
                                                  Scheme = "Bearer",
                                                  BearerFormat = "JWT",
                                                  Description =
                                                      "JWT Authorization header using 'Bearer' scheme.",
                                                  Type = SecuritySchemeType.ApiKey
                                              });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseReDoc(c =>
            {
                c.RoutePrefix = "docs";
                c.SpecUrl("/swagger/v1/swagger.json");
                c.EnableUntrustedSpec();
                c.ScrollYOffset(10);
                c.ExpandResponses("200,201");
                c.RequiredPropsFirst();
                c.PathInMiddlePanel();
                c.HideLoading();
                c.NativeScrollbars();
            });

            return app;
        }
    }
}