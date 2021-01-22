//  <copyright file="AuthService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using BCrypt.Net;

    using Contracts;

    using Core.Constants;
    using Core.Contracts;
    using Core.Events.Auth;

    using Data.Entities;

    using Identity.Models.Authentication;

    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// </summary>
    /// <seealso cref="Identity.Services.Contracts.IAuthService" />
    public class AuthService : IAuthService
    {
        /// <summary>
        ///     The JWT service
        /// </summary>
        private readonly IJwtService jwtService;

        /// <summary>
        ///     The user manager
        /// </summary>
        private readonly UserManager<User> userManager;

        private readonly IEventProducer<UserRegisterEvent> userRegisterEvent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AuthService" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="jwtService">The JWT service.</param>
        /// <param name="userRegisterEvent"></param>
        public AuthService(UserManager<User> userManager, IJwtService jwtService,
                           IEventProducer<UserRegisterEvent> userRegisterEvent)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.userRegisterEvent = userRegisterEvent;
        }

        /// <summary>
        ///     Revokes the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        public async Task<bool> RevokeToken(string token, string ipAddress)
        {
            User user = this.userManager.Users
                            .FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) throw new Exception();

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) throw new Exception();

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIP = ipAddress;
            await this.userManager.UpdateAsync(user);

            return true;
        }

        /// <summary>
        ///     Creates the JWT security stamp.
        /// </summary>
        /// <param name="userSecurityStamp">The user security stamp.</param>
        /// <param name="userSalt">The user salt.</param>
        /// <returns></returns>
        public string CreateJwtSecurityStamp(string userSecurityStamp, string userSalt)
        {
            return BCrypt.HashPassword(userSecurityStamp, userSalt);
        }

        /// <summary>
        ///     Validates the JWT security stamp.
        /// </summary>
        /// <param name="jwtSecurityStamp">The JWT security stamp.</param>
        /// <param name="userSecurityStamp">The user security stamp.</param>
        /// <returns></returns>
        public bool ValidateJwtSecurityStamp(string jwtSecurityStamp, string userSecurityStamp)
        {
            return BCrypt.Verify(userSecurityStamp, jwtSecurityStamp);
        }

        /// <summary>
        ///     Authenticates the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        public async Task<TokensModel> Authenticate(SignInRequest model, string ipAddress)
        {
            User user = await this.userManager.FindByNameAsync(model.UserName);

            if (user == null) throw new Exception();

            if (!await this.userManager.CheckPasswordAsync(user, model.Password)) throw new Exception();

            if (user.RefreshTokens.Any(t => t.IsActive))
                await this.RevokeToken(user.RefreshTokens.First(t => t.IsActive).Token, ipAddress);

            await this.userManager.UpdateSecurityStampAsync(user);
            string jwtSecurityStamp = this.CreateJwtSecurityStamp(user.SecurityStamp, user.Salt);

            List<Claim> claims = new List<Claim>
                                 {
                                     new Claim(AuthConstants.UserIdClaimType, user.Id.ToString()),
                                     new Claim(AuthConstants.JwtSecurityStamp, jwtSecurityStamp)
                                 };

            IList<Claim> userClaims = await this.userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            IList<string> userRoles = await this.userManager.GetRolesAsync(user);

            foreach (string userRole in userRoles) claims.Add(new Claim(ClaimTypes.Role, userRole));

            RefreshToken refreshToken = this.jwtService.GetRefreshToken(jwtSecurityStamp, ipAddress);
            DateTime accessTokenExpires = DateTime.UtcNow.AddMinutes(15);
            string accessToken = this.jwtService.GetAccessToken(claims, accessTokenExpires);

            user.RefreshTokens ??= new List<RefreshToken>();

            user.RefreshTokens.Add(refreshToken);
            await this.userManager.UpdateAsync(user);

            return new TokensModel
                   {
                       AccessToken = accessToken,
                       RefreshToken = refreshToken.Token,
                       ExpiresAt = accessTokenExpires
                   };
        }

        /// <summary>
        ///     Refreshes the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        /// </exception>
        public async Task<TokensModel> RefreshToken(string token, string ipAddress)
        {
            User user = this.userManager.Users
                            .FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) throw new Exception();

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!this.ValidateJwtSecurityStamp(refreshToken.SecurityStamp, user.SecurityStamp)
             && !refreshToken.IsActive) throw new Exception();

            user.SecurityStamp = Guid.NewGuid().ToString("D");
            string jwtSecurityStamp = this.CreateJwtSecurityStamp(user.SecurityStamp, user.Salt);

            List<Claim> claims = new List<Claim>
                                 {
                                     new Claim(AuthConstants.UserIdClaimType, user.Id.ToString()),
                                     new Claim(AuthConstants.JwtSecurityStamp, jwtSecurityStamp)
                                 };

            RefreshToken newRefreshToken = this.jwtService.GetRefreshToken(jwtSecurityStamp, ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIP = ipAddress;
            refreshToken.ReplacedBy = newRefreshToken.ID;

            user.RefreshTokens.Add(newRefreshToken);
            await this.userManager.UpdateAsync(user);

            DateTime accessTokenExpires = DateTime.UtcNow.AddMinutes(15);
            string newAccessToken = this.jwtService.GetAccessToken(claims, accessTokenExpires);

            return new TokensModel
                   {
                       AccessToken = newAccessToken,
                       RefreshToken = newRefreshToken.Token,
                       ExpiresAt = accessTokenExpires
                   };
        }
    }
}