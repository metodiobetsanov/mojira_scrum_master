//  <copyright file="SubscriptionsController.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public IActionResult Subscribe()
        {
            return this.Ok(nameof(this.Subscribe));
        }

        [HttpPost]
        [Authorize]
        public IActionResult UnSubscribe()
        {
            return this.Ok(nameof(this.UnSubscribe));
        }
    }
}