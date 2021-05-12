// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Common
{
    using System;

    /// <summary>
    /// Enum of log levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug messages that are hidden unless the verbose setting is enabled.
        /// </summary>
        DEBUG,

        /// <summary>
        /// Messages that indicate something worth note.
        /// </summary>
        INFO,

        /// <summary>
        /// Messages that happen without input, usually started by something external.
        /// </summary>
        LOG,

        /// <summary>
        /// Messages that indicate non-breaking errors that are worked around automatically.
        /// </summary>
        WARNING,

        /// <summary>
        /// Messages that indicate errors which force the program to stop.
        /// </summary>
        ERROR,
    }

    /// <summary>
    /// Wrapper of Console.WriteLine to log messages with levels.
    /// </summary>
    internal class Output
    {
        /// <summary>
        /// Gets or sets a value indicating whether to display debug messages.
        /// </summary>
        public static bool Verbose { get; set; }

        /// <summary>
        /// Outputs to Console with the level prepended.
        /// </summary>
        /// <param name="level">Level of output.</param>
        /// <param name="obj">The content to output.</param>
        public static void Log(LogLevel level, object obj)
        {
            if (level != LogLevel.DEBUG || (level == LogLevel.DEBUG && Verbose))
            {
                Console.WriteLine($"[{level}] {obj}");
            }
        }
    }
}