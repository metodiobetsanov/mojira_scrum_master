//  <copyright file="IAuthService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Services.Contracts
{
    using System.Threading.Tasks;

    using Identity.Models.Authentication;

    public interface IAuthService
    {
        Task<TokensModel> Authenticate(SignInRequest model, string ipAddress);

        Task<TokensModel> RefreshToken(string token, string ipAddress);

        Task<bool> RevokeToken(string token, string ipAddress);

        string CreateJwtSecurityStamp(string userSecurityStamp, string userSalt);

        bool ValidateJwtSecurityStamp(string jwtSecurityStamp, string userSecurityStamp);
    }
}