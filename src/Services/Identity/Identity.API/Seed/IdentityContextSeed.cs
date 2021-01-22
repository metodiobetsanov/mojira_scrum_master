//  <copyright file="IdentityContextSeed.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API.Seed
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BCrypt.Net;

    using Core.Constants;

    using Data;
    using Data.Entities;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;

    using Polly;
    using Polly.Retry;

    /// <summary>
    ///     Identity API Context Seeded
    /// </summary>
    public class IdentityContextSeed
    {
        /// <summary>
        ///     The password hasher
        /// </summary>
        private readonly IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        /// <summary>
        ///     Seeds the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task SeedAsync(IdentityAPIContext context, ILogger<IdentityContextSeed> logger)
        {
            AsyncRetryPolicy policy = this.CreatePolicy(logger, nameof(IdentityContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                logger.LogInformation("Executing IdentityContextSeed");
                bool contextChanged = false;

                // Check if MojiraRole exists, if not creates it
                logger.LogInformation("IdentityContextSeed: check for role: " + AuthConstants.MojiraRole.ToUpper());
                if (!context.Roles.Any(u => u.NormalizedName == AuthConstants.MojiraRole.ToUpper()))
                {
                    logger.LogInformation("IdentityContextSeed: create role: " + AuthConstants.MojiraRole.ToUpper());
                    Role role = new Role
                                {
                                    Id = Guid.NewGuid(),
                                    Name = AuthConstants.MojiraRole,
                                    NormalizedName = AuthConstants.MojiraRole.ToUpper(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                                };
                    await context.Roles.AddAsync(role);
                    contextChanged = true;
                }

                // Check if AdminRole exists, if not creates it
                logger.LogInformation("IdentityContextSeed: check for role: " + AuthConstants.AdminRole.ToUpper());
                if (!context.Roles.Any(u => u.NormalizedName == AuthConstants.AdminRole.ToUpper()))
                {
                    logger.LogInformation("IdentityContextSeed: create for role: " + AuthConstants.AdminRole.ToUpper());
                    Role role = new Role
                                {
                                    Id = Guid.NewGuid(),
                                    Name = AuthConstants.AdminRole,
                                    NormalizedName = AuthConstants.AdminRole.ToUpper(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                                };
                    await context.Roles.AddAsync(role);
                    contextChanged = true;
                }

                // Check if ProjectOwnerRole exists, if not creates it
                logger.LogInformation(
                    "IdentityContextSeed: check for role: " + AuthConstants.ProjectOwnerRole.ToUpper());
                if (!context.Roles.Any(u => u.NormalizedName == AuthConstants.ProjectOwnerRole.ToUpper()))
                {
                    logger.LogInformation(
                        "IdentityContextSeed: create role: " + AuthConstants.ProjectOwnerRole.ToUpper());
                    Role role = new Role
                                {
                                    Id = Guid.NewGuid(),
                                    Name = AuthConstants.ProjectOwnerRole,
                                    NormalizedName = AuthConstants.ProjectOwnerRole.ToUpper(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                                };
                    await context.Roles.AddAsync(role);
                    contextChanged = true;
                }

                // Check if TeamLeadRole exists, if not creates it
                logger.LogInformation("IdentityContextSeed: check for role: " + AuthConstants.TeamLeadRole.ToUpper());
                if (!context.Roles.Any(u => u.NormalizedName == AuthConstants.TeamLeadRole.ToUpper()))
                {
                    logger.LogInformation("IdentityContextSeed: create role: " + AuthConstants.TeamLeadRole.ToUpper());
                    Role role = new Role
                                {
                                    Id = Guid.NewGuid(),
                                    Name = AuthConstants.TeamLeadRole,
                                    NormalizedName = AuthConstants.TeamLeadRole.ToUpper(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                                };
                    await context.Roles.AddAsync(role);
                    contextChanged = true;
                }

                // Check if UserRole exists, if not creates it
                logger.LogInformation("IdentityContextSeed: check for role: " + AuthConstants.UserRole.ToUpper());
                if (!context.Roles.Any(u => u.NormalizedName == AuthConstants.UserRole.ToUpper()))
                {
                    logger.LogInformation("IdentityContextSeed: create role: " + AuthConstants.UserRole.ToUpper());
                    Role role = new Role
                                {
                                    Id = Guid.NewGuid(),
                                    Name = AuthConstants.UserRole,
                                    NormalizedName = AuthConstants.UserRole.ToUpper(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                                };
                    await context.Roles.AddAsync(role);
                    contextChanged = true;
                }

                // Check if ScrumMasterRole exists, if not creates it
                logger.LogInformation("IdentityContextSeed: check for role: " +
                                      AuthConstants.ScrumMasterRole.ToUpper());
                if (!context.Roles.Any(u => u.NormalizedName == AuthConstants.ScrumMasterRole.ToUpper()))
                {
                    logger.LogInformation("IdentityContextSeed: create role: " +
                                          AuthConstants.ScrumMasterRole.ToUpper());
                    Role role = new Role
                                {
                                    Id = Guid.NewGuid(),
                                    Name = AuthConstants.ScrumMasterRole,
                                    NormalizedName = AuthConstants.ScrumMasterRole.ToUpper(),
                                    ConcurrencyStamp = Guid.NewGuid().ToString("D")
                                };
                    await context.Roles.AddAsync(role);
                    contextChanged = true;
                }

                // Check if main user exists, if not creates it
                logger.LogInformation("IdentityContextSeed: check if main user exists");
                if (!context.Users.Any(u => u.NormalizedUserName == "MOJIRA"))
                {
                    logger.LogInformation("IdentityContextSeed: create main user");
                    User user = new User();
                    user.Id = Guid.NewGuid();
                    user.UserName = "mojira";
                    user.NormalizedUserName = "MOJIRA";
                    user.Salt = BCrypt.GenerateSalt();
                    user.PasswordHash = this.passwordHasher.HashPassword(user, "Sup3rP@ssw0rd");

                    await context.Users.AddAsync(user);
                    contextChanged = true;
                }

                Guid userId = context.Users.First(u => u.NormalizedUserName == "MOJIRA").Id;
                Guid roleId = context.Roles.First(u => u.NormalizedName == AuthConstants.MojiraRole.ToUpper()).Id;

                // Check if main user has MojiraRole assigned, if not creates it
                logger.LogInformation("IdentityContextSeed: check if main user has " +
                                      AuthConstants.MojiraRole.ToUpper());
                if (!context.UserRoles.Any(x => x.UserId == userId && x.RoleId == roleId))
                {
                    logger.LogInformation("IdentityContextSeed: assign main user to " +
                                          AuthConstants.MojiraRole.ToUpper());
                    IdentityUserRole<Guid> userRole = new IdentityUserRole<Guid>();
                    userRole.UserId = userId;
                    userRole.RoleId = roleId;

                    await context.UserRoles.AddAsync(userRole);
                    contextChanged = true;
                }

                // If context has changes, save it
                if (contextChanged)
                    await context.SaveChangesAsync();
            });
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<IdentityContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().WaitAndRetryAsync(
                retries,
                retry => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(
                        exception,
                        "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
        }
    }
}