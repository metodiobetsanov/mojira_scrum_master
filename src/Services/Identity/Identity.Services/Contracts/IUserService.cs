//  <copyright file="IUserService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.Services.Contracts
{
    using System.Threading.Tasks;

    using Identity.Models.User;

    using Models.User;

    public interface IUserService
    {
        Task<UserBaseModel> CreateUserAsync(CreateUserRequest model);
    }
}