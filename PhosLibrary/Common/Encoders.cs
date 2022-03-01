// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains the paths to any dependency encoders.
    /// </summary>
    internal class Encoders
    {
        private static readonly string DependenciesFolder = Path.Join(Path.Join(Directory.GetCurrentDirectory(), "dependencies"));
        private static readonly string AdpcmPath = Path.Join(DependenciesFolder, "AdpcmEncode.exe");
        private static readonly string AtomEncPath = Path.Join(DependenciesFolder, "CriEncoder", "criatomencd.exe");

        /// <summary>
        /// Gets the expected file path of AdpcmEncode. Throws a <c>FileNotFoundException</c> if missing.
        /// </summary>
        /// <returns>File path of AdpcmEncode.</returns>
        public static string GetAdpcmPath()
        {
            if (!File.Exists(AdpcmPath))
            {
                throw new FileNotFoundException("AdpcmEncode.exe could not be found.", AdpcmPath);
            }

            return AdpcmPath;
        }

        /// <summary>
        /// Gets the expected file path of criatomencd. Throws a <c>FileNotFoundException</c> if missing.
        /// </summary>
        /// <returns>File path of criatomencd.</returns>
        public static string GetAtomPath()
        {
            // TODO: Check if required atom dlls also exist.
            if (!File.Exists(AtomEncPath))
            {
                throw new FileNotFoundException("criatomencd.exe could not be found.", AtomEncPath);
            }

            return AtomEncPath;
        }
    }
}