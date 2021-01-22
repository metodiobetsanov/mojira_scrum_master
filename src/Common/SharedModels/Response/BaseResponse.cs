//  <copyright file="BaseResponse.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace SharedModels.Response
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     Mojira Base Response Model
    /// </summary>
    [DataContract]
    public class BaseResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseResponse" /> class.
        /// </summary>
        public BaseResponse()
        {
            this.ResponseHeader = new ResponseHeader();
        }

        /// <summary>
        ///     Gets or sets the response header.
        /// </summary>
        /// <value>
        ///     The response header.
        /// </value>
        [DataMember(Name = "responseHeader")]
        public ResponseHeader ResponseHeader { get; set; }
    }
}