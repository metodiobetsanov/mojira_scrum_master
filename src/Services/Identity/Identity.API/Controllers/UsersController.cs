//  <copyright file="UserController.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Models.User;

    using Services.Contracts;

    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            return this.Ok(nameof(this.GetAllUsers));
        }

        [HttpGet("{userId}")]
        [Authorize]
        public IActionResult GetUser([FromRoute] string userId)
        {
            return this.Ok(nameof(this.GetUser) + " => " + userId);
        }

        [HttpPost("create")]
        //[Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            await this.userService.CreateUserAsync(request);

            return this.Ok(nameof(this.CreateUser));
        }

        [HttpPut("{userId}")]
        [Authorize]
        public IActionResult UpdateUser([FromRoute] string userId)
        {
            return this.Ok(nameof(this.UpdateUser) + " => " + userId);
        }
    }
}