//  <copyright file="CustomAuthenticationExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Extensions
{
    using System;
    using System.Security.Cryptography;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    using Settings;

    public static class CustomAuthenticationExtension
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,
                                                                 IConfiguration configuration)
        {
            JwtSettings jwtTokenConfig = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddSingleton(jwtTokenConfig);

            services.AddAuthentication(x => { x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; })
                    .AddJwtBearer(x =>
                    {
                        using (RSA rsa = RSA.Create())
                        {
                            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(jwtTokenConfig.JwtPublicKey),
                                                           out _);

                            x.RequireHttpsMetadata = false;
                            x.TokenValidationParameters = new TokenValidationParameters
                                                          {
                                                              ValidateIssuer = true,
                                                              ValidIssuer = jwtTokenConfig.Issuer,
                                                              ValidateIssuerSigningKey = true,
                                                              IssuerSigningKey = new RsaSecurityKey(rsa),
                                                              ValidAudience = jwtTokenConfig.Audience,
                                                              ValidateAudience = true,
                                                              ValidateLifetime = true,
                                                              ClockSkew = TimeSpan.FromMinutes(1)
                                                          };
                        }
                    });

            return services;
        }

        /// <summary>
        ///     Uses the custom authentication.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}