//  <copyright file="RefreshToken.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Data.Entities
{
    using System;

    public class RefreshToken
    {
        public Guid ID { get; set; }

        public Guid UserID { get; set; }
        public virtual User User { get; set; }

        public string Token { get; set; }

        public string SecurityStamp { get; set; }

        public bool IsActive => !this.IsRevoked && !this.IsExpired;

        public DateTime Created { get; set; }

        public string CreatedByIP { get; set; }

        public DateTime Expires { get; set; }

        public DateTime? Revoked { get; set; }

        public string RevokedByIP { get; set; }

        public Guid ReplacedBy { get; set; }

        public bool IsExpired => DateTime.UtcNow >= this.Expires;

        public bool IsRevoked => this.Revoked != null;
    }
}