//  <copyright file="ResetPasswordRequest.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Models.Authentication
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ResetPasswordRequest
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}