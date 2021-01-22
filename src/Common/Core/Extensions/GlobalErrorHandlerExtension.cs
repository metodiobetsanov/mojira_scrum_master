//  <copyright file="GlobalErrorHandlerExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Extensions
{
    using System.Net;

    using BaseResponseModel;

    using Exception;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    public static class GlobalErrorHandlerExtension
    {
        public static void UseGlobalErrorHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    logger.LogDebug("Executing GlobalErrorHandler");
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    BaseResponse response = new BaseResponse
                                            {
                                                ResponseHeader = new ResponseHeader
                                                                 {
                                                                     StatusCode =
                                                                         (int) HttpStatusCode.InternalServerError,
                                                                     StatusMessage = "Internal Server Error",
                                                                     ErrorMessage = "Something went wrong"
                                                                 }
                                            };

                    IExceptionHandlerFeature exceptionContext = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionContext?.Error != null)
                    {
                        logger.LogDebug("GlobalErrorHandler: processing an exception of type: " +
                                        exceptionContext.Error.GetType());

                        if (exceptionContext.Error is HttpException exception)
                        {
                            context.Response.StatusCode = exception.StatusCode;
                            response.ResponseHeader.StatusCode = exception.StatusCode;
                            response.ResponseHeader.ErrorMessage = exception.DisplayMessage;

                            logger.Log(exception.LogLevel, exception, exception.DebugMessage);
                        }
                        else
                        {
                            logger.LogCritical(exceptionContext.Error,
                                               response.ResponseHeader.StatusMessage);
                        }
                    }

                    logger.LogDebug("Executed GlobalErrorHandler");
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                });
            });
        }
    }
}