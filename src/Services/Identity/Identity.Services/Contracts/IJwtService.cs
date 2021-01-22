//  <copyright file="IJwtService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    using Data.Entities;

    public interface IJwtService
    {
        string GetAccessToken(List<Claim> claims, DateTime expiresAt);

        RefreshToken GetRefreshToken(string securityStamp, string ipAddress);
    }
}