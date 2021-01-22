//  <copyright file="User.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Data.Entities
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser<Guid>
    {
        public string Salt { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public virtual string FullName => $"{this.LastName}, {this.FirstName} {this.MiddleName.Substring(0, 1)}";

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}