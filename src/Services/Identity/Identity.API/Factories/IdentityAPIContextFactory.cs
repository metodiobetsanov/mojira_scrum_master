//  <copyright file="IdentityAPIContextFactory.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API.Factories
{
    using System.IO;

    using Data;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    ///     Application Database Context Factory
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory{Identity.Data.IdentityAPIContext}" />
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<IdentityAPIContext>
    {
        /// <summary>
        ///     Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>
        ///     An instance of <typeparamref name="TContext" />.
        /// </returns>
        public IdentityAPIContext CreateDbContext(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                                        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                                        .AddJsonFile("appsettings.json")
                                        .AddEnvironmentVariables()
                                        .Build();

            DbContextOptionsBuilder<IdentityAPIContext> optionsBuilder =
                new DbContextOptionsBuilder<IdentityAPIContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], o => o.MigrationsAssembly("Identity.Data"));

            return new IdentityAPIContext(optionsBuilder.Options);
        }
    }
}