//  <copyright file="AutoMapping.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Services.AutoMapper
{
    using Data.Entities;

    using global::AutoMapper;

    using Models.User;

    public static class AutoMapping
    {
        private static bool initialized;

        private static MapperConfiguration config;

        public static MapperConfiguration Config()
        {
            if (!initialized)
            {
                initialized = true;

                config = new MapperConfiguration(cfg => { cfg.CreateMap<User, UserBaseModel>(); });
            }

            config.AssertConfigurationIsValid();
            return config;
        }
    }
}