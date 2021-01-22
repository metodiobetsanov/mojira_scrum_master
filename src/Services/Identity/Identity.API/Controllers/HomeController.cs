//  <copyright file="HomeController.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Home Controller
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// Index Route
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return new RedirectResult("~/docs");
        }
    }
}