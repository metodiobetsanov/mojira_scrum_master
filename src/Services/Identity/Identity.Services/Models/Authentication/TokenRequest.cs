//  <copyright file="TokenRequest.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Models.Authentication
{
    using System.Runtime.Serialization;

    [DataContract]
    public class TokenRequest
    {
        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}