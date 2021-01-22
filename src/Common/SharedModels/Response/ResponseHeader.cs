//  <copyright file="ResponseHeader.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace SharedModels.Response
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     Response Header Model
    /// </summary>
    [DataContract]
    public class ResponseHeader
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ResponseHeader" /> class.
        /// </summary>
        public ResponseHeader()
        {
            this.StatusCode = 200;
            this.StatusMessage = "OK";
        }

        /// <summary>
        ///     Gets or sets the status code.
        /// </summary>
        /// <value>
        ///     The status code.
        /// </value>
        [DataMember(Name = "statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the status message.
        /// </summary>
        /// <value>
        ///     The status message.
        /// </value>
        [DataMember(Name = "statusMessage")]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     Gets or sets the error message.
        /// </summary>
        /// <value>
        ///     The error message.
        /// </value>
        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
    }
}