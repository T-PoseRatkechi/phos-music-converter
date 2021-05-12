// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Interfaces
{
    /// <summary>
    /// Interface that all game Music Builders implement.
    /// </summary>
    internal interface IMusicBuilder
    {
        /// <summary>
        /// Generates a music build in <paramref name="outputDir"/>.
        /// </summary>
        /// <param name="outputDir">Directory to output music build to.</param>
        /// <param name="useLow">Whether to use less resource intensive processes for generating builds.</param>
        /// <param name="verbose">Output more console comments. Meant for debugging purposes.</param>
        void GenerateBuild(string outputDir, bool useLow, bool verbose);
    }
}