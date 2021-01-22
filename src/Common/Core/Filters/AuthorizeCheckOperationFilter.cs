//  <copyright file="AuthorizeCheckOperationFilter.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Filters
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.OpenApi.Models;

    using Swashbuckle.AspNetCore.SwaggerGen;

    /// <summary>
    ///     Authorize Check Operation Filter
    /// </summary>
    /// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" />
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        /// <summary>
        ///     Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="context">The context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            bool hasAuthorize = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (!hasAuthorize) return;

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            OpenApiSecurityScheme bearer = new OpenApiSecurityScheme
                                           {
                                               Reference = new OpenApiReference
                                                           { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                                           };

            List<string> requirements = new List<string>
                                        {
                                            "Registered"
                                        };

            string role = context.MethodInfo
                                 .GetCustomAttributes(true)
                                 .OfType<AuthorizeAttribute>()
                                 .FirstOrDefault(x => !string.IsNullOrEmpty(x.Roles))
                                 ?.Roles;

            if (!string.IsNullOrEmpty(role)) requirements.Add(role + " Role");

            operation.Security = new List<OpenApiSecurityRequirement>
                                 {
                                     new OpenApiSecurityRequirement
                                     {
                                         [bearer] = requirements.ToArray()
                                     }
                                 };
        }
    }
}