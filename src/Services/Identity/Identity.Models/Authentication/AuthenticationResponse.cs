//  <copyright file="AuthenticationResponse.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Models.Authentication
{
    using System.Runtime.Serialization;

    using SharedModels.Response;

    [DataContract]
    public class AuthenticationResponse : BaseResponse
    {
        [DataMember(Name = "tokens")]
        public TokensModel Tokens { get; set; }
    }
}