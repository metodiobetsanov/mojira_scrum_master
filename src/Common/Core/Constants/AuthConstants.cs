//  <copyright file="AuthConstants.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Constants
{
    /// <summary>
    ///     Authentication Constants
    /// </summary>
    public static class AuthConstants
    {
        /// <summary>
        ///     The user identifier claim type
        /// </summary>
        public const string UserIdClaimType = "IDENTITY";

        /// <summary>
        ///     The JWT security stamp
        /// </summary>
        public const string JwtSecurityStamp = "SECURITYSTAMP";

        /// <summary>
        ///     The mojira role
        /// </summary>
        public const string MojiraRole = "Mojira";

        /// <summary>
        ///     The admin role
        /// </summary>
        public const string AdminRole = "Admin";

        /// <summary>
        ///     The project owner role
        /// </summary>
        public const string ProjectOwnerRole = "ProjectOwner";

        /// <summary>
        ///     The team lead role
        /// </summary>
        public const string TeamLeadRole = "TeamLead";

        /// <summary>
        ///     The user role
        /// </summary>
        public const string UserRole = "User";

        /// <summary>
        ///     The scrum master role
        /// </summary>
        public const string ScrumMasterRole = "ScrumMaster";
    }
}