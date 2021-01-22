//  <copyright file="TokensModel.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Models.Authentication
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class TokensModel
    {
        [DataMember(Name = "accessToken")]
        public string AccessToken { get; set; }

        [DataMember(Name = "refreshToken")]
        public string RefreshToken { get; set; }

        [DataMember(Name = "expiresAt")]
        public DateTime ExpiresAt { get; set; }
    }
}