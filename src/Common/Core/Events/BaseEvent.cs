//  <copyright file="BaseEvent.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Events
{
    using System;

    /// <summary>
    ///     Base Event Model
    /// </summary>
    public class BaseEvent
    {
        public Guid RequestId { get; set; }
    }
}