//  <copyright file="HttpException.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Exception
{
    using System;
    using System.Net;

    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Mojira HTTP Exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HttpException : Exception
    {
        /// <summary>
        ///     The internal server error message
        /// </summary>
        private const string InternalServerError = "Internal Server Error";

        /// <summary>
        ///     The debug message
        /// </summary>
        private readonly string debugMessage;

        /// <summary>
        ///     The display message
        /// </summary>
        private readonly string displayMessage;

        /// <summary>
        ///     The log level
        /// </summary>
        private readonly LogLevel logLevel;

        /// <summary>
        ///     The status code
        /// </summary>
        private readonly HttpStatusCode statusCode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        public HttpException()
        {
            this.statusCode = HttpStatusCode.InternalServerError;
            this.debugMessage = InternalServerError;
            this.displayMessage = InternalServerError;
            this.logLevel = LogLevel.Error;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="debugMessage">The debug message.</param>
        /// <param name="logLevel"></param>
        public HttpException(HttpStatusCode code, string debugMessage, LogLevel logLevel) : base(debugMessage)
        {
            this.statusCode = code;
            this.debugMessage = debugMessage;
            this.displayMessage = InternalServerError;
            this.logLevel = logLevel;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="debugMessage">The debug message.</param>
        /// <param name="displayMessage">The display message.</param>
        /// <param name="logLevel"></param>
        public HttpException(HttpStatusCode code, string debugMessage, string displayMessage, LogLevel logLevel) : base(
            debugMessage)
        {
            this.statusCode = code;
            this.debugMessage = debugMessage;
            this.displayMessage = displayMessage;
            this.logLevel = logLevel;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="debugMessage">The debug message.</param>
        /// <param name="logLevel"></param>
        /// <param name="inner">The inner.</param>
        public HttpException(HttpStatusCode code, string debugMessage, LogLevel logLevel, Exception inner) : base(
            debugMessage, inner)
        {
            this.statusCode = code;
            this.debugMessage = debugMessage;
            this.displayMessage = InternalServerError;
            this.logLevel = logLevel;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="debugMessage">The debug message.</param>
        /// <param name="displayMessage">The display message.</param>
        /// <param name="logLevel"></param>
        /// <param name="inner">The inner.</param>
        public HttpException(HttpStatusCode code, string debugMessage, string displayMessage, LogLevel logLevel,
                             Exception inner) : base(
            debugMessage, inner)
        {
            this.statusCode = code;
            this.debugMessage = debugMessage;
            this.displayMessage = displayMessage;
            this.logLevel = logLevel;
        }

        /// <summary>
        ///     Gets the log level.
        /// </summary>
        /// <value>
        ///     The log level.
        /// </value>
        public LogLevel LogLevel => this.logLevel;

        /// <summary>
        ///     Gets the status code.
        /// </summary>
        /// <value>
        ///     The status code.
        /// </value>
        public int StatusCode => (int) this.statusCode;

        /// <summary>
        ///     Gets the debug message.
        /// </summary>
        /// <value>
        ///     The debug message.
        /// </value>
        public string DebugMessage => "Debug: " + this.debugMessage;

        /// <summary>
        ///     Gets the display message.
        /// </summary>
        /// <value>
        ///     The display message.
        /// </value>
        public string DisplayMessage => "Service Unavailable: " + this.displayMessage;
    }
}