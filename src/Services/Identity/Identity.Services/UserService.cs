//  <copyright file="UserService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Service.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using BCrypt.Net;

    using Core.Constants;
    using Core.Contracts;
    using Core.Events.Auth;

    using Data.Entities;

    using Identity.Services.Contracts;
    using Identity.Services.Models.User;

    using Microsoft.AspNetCore.Identity;

    using Models.User;

    public class UserService : IUserService
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IEventProducer<UserRegisterEvent> userRegisteredEventProducer;

        public UserService(UserManager<User> userManager, IMapper mapper,
                           IEventProducer<UserRegisterEvent> userRegisteredEventProducer)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.userRegisteredEventProducer = userRegisteredEventProducer;
        }

        public async Task<UserBaseModel> CreateUserAsync(CreateUserRequest model)
        {
            User user = new User { Salt = BCrypt.GenerateSalt(), UserName = model.Username, Email = model.Email };

            IdentityResult createResult = await this.userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
                throw new Exception(createResult.Errors.SelectMany(e => e.Description).ToString());

            string emailConfirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

            this.userRegisteredEventProducer.PublishEvent(EventBusConstants.MailSendQueue, new UserRegisterEvent
                                                              {
                                                                  RequestId = Guid.NewGuid(),
                                                                  UserName = user.UserName,
                                                                  Email = user.Email
                                                              });

            return new UserBaseModel();
        }
    }
}