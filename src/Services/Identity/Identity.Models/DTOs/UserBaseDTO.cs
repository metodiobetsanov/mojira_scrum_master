﻿//  <copyright file="UserBaseDTO.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Models.DTOs
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserBaseDTO
    {
        [DataMember(Name = "id")] public Guid Id { get; set; }

        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "fullName")]
        public string FullName { get; set; }
    }
}