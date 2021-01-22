//  <copyright file="JwtService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Service.Services
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;

    using Core.Settings;

    using Data.Entities;

    using Identity.Services.Contracts;

    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// </summary>
    /// <seealso cref="Identity.Services.Contracts.IJwtService" />
    public class JwtService : IJwtService
    {
        /// <summary>
        ///     The JWT settings
        /// </summary>
        private readonly JwtSettings jwtSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JwtService" /> class.
        /// </summary>
        /// <param name="jwtSettings">The JWT settings.</param>
        public JwtService(JwtSettings jwtSettings)
        {
            this.jwtSettings = jwtSettings;
        }

        /// <summary>
        ///     Create the access token.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="expiresAt">The expires at.</param>
        /// <returns></returns>
        public string GetAccessToken(List<Claim> claims, DateTime expiresAt)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(this.jwtSettings.JwtPrivateKey), out _);
            SigningCredentials signingCredentials =
                new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                };

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                audience: this.jwtSettings.Audience,
                issuer: this.jwtSettings.Issuer,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                claims: claims,
                signingCredentials: signingCredentials);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }

        /// <summary>
        ///     Create the refresh token.
        /// </summary>
        /// <param name="securityStamp">The security stamp.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns></returns>
        public RefreshToken GetRefreshToken(string securityStamp, string ipAddress)
        {
            using (RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                       {
                           Token = Convert.ToBase64String(randomBytes),
                           SecurityStamp = securityStamp,
                           Expires = DateTime.UtcNow.AddHours(6),
                           Created = DateTime.UtcNow,
                           CreatedByIP = ipAddress
                       };
            }
        }
    }
}