//  <copyright file="AutoMapping.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.Services.AutoMapper
{
    using global::AutoMapper;

    public static class AutoMapping
    {
        private static bool initialized;

        private static MapperConfiguration config;

        public static MapperConfiguration Config()
        {
            if (!initialized)
            {
                initialized = true;

                config = new MapperConfiguration(cfg => { });
            }

            config.AssertConfigurationIsValid();
            return config;
        }
    }
}