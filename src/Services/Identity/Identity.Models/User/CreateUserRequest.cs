//  <copyright file="CreateUserRequest.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Models.User
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CreateUserRequest
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}