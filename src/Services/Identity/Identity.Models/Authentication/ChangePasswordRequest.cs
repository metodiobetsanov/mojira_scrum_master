//  <copyright file="ChangePasswordRequest.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Models.Authentication
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ChangePasswordRequest
    {
        [DataMember(Name = "oldPassword")]
        public string OldPassword { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}