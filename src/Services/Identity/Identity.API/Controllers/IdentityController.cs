//  <copyright file="IdentityController.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API.Controllers
{
    using System.Threading.Tasks;

    using Core.BaseResponseModel;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Models.Authentication;

    using Services.Contracts;

    /// <summary>
    ///     Authentication Controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        ///     The authentication service
        /// </summary>
        private readonly IAuthService authService;

        /// <summary>
        ///     The logger
        /// </summary>
        private readonly ILogger<IdentityController> logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IdentityController" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="authService">The authentication service.</param>
        public IdentityController(ILogger<IdentityController> logger, IAuthService authService)
        {
            this.logger = logger;
            this.authService = authService;
        }

        /// <summary>
        ///     Signs in the User.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> SignIn([FromBody] SignInRequest request)
        {
            this.logger.LogDebug("Executing 'SignIn' method");
            AuthenticationResponse response = new AuthenticationResponse();

            response.Tokens = await this.authService.Authenticate(request, this.GetIpAddress());

            this.logger.LogDebug("Executed 'SignIn' method");
            return this.Ok(response);
        }

        /// <summary>
        ///     Resets the User's password.
        /// </summary>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            this.logger.LogDebug("Executing 'ResetPassword' method");
            this.logger.LogDebug("Executed 'ResetPassword' method");
            return this.Ok(nameof(this.ResetPassword));
        }

        /// <summary>
        ///     Change the User's password.
        /// </summary>
        /// <returns></returns>
        [HttpPost("change-password")]
        [Authorize(Roles = "User")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            this.logger.LogDebug("Executing 'ChangePassword' method");
            this.logger.LogDebug("Executed 'ChangePassword' method");
            return this.Ok(nameof(this.ChangePassword));
        }

        /// <summary>
        ///     Confirms User's email(get token).
        /// </summary>
        /// <returns></returns>
        [HttpGet("confirm-email")]
        [Authorize]
        public IActionResult GetConfirmEmailToken()
        {
            this.logger.LogDebug("Executing 'GetConfirmEmailToken' method");
            this.logger.LogDebug("Executed 'GetConfirmEmailToken' method");
            return this.Ok(nameof(this.ConfirmEmail));
        }

        /// <summary>
        ///     Confirms User's email(validate token).
        /// </summary>
        /// <returns></returns>
        [HttpPost("confirm-email")]
        [Authorize]
        public IActionResult ConfirmEmail([FromBody] TokenRequest request)
        {
            this.logger.LogDebug("Executing 'ConfirmEmail' method");
            this.logger.LogDebug("Executed 'ConfirmEmail' method");
            return this.Ok(nameof(this.ConfirmEmail));
        }

        /// <summary>
        ///     Refreshes the User's access token.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthenticationResponse>> RefreshToken([FromBody] TokenRequest request)
        {
            this.logger.LogDebug("Executing 'RefreshToken' method");

            AuthenticationResponse response = new AuthenticationResponse();
            response.Tokens = await this.authService.RefreshToken(request.Token, this.GetIpAddress());

            this.logger.LogDebug("Executed 'RefreshToken' method");
            return this.Ok(response);
        }

        /// <summary>
        ///     Revokes the User's refresh token.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<ActionResult<BaseResponse>> RevokeToken([FromBody] TokenRequest request)
        {
            this.logger.LogDebug("Executing 'RevokeToken' method");

            BaseResponse response = new BaseResponse();
            await this.authService.RevokeToken(request.Token, this.GetIpAddress());

            this.logger.LogDebug("Executed 'RevokeToken' method");
            return this.Ok(response);
        }

        /// <summary>
        ///     Gets the ip address.
        /// </summary>
        /// <returns>The IP as string</returns>
        private string GetIpAddress()
        {
            if (this.Request.Headers.ContainsKey("X-Forwarded-For"))
                return this.Request.Headers["X-Forwarded-For"];

            return this.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}