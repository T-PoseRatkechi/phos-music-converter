// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Builders.Music
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using PhosLibrary.Common;
    using PhosLibrary.Common.Logging;

    /// <summary>
    /// Music Builder for Persona 3 FES and Persona 4.
    /// </summary>
    internal class BuilderP3F : BuilderADX
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderP3F"/> class.
        /// </summary>
        public BuilderP3F()
            : base("Persona 3 FES/Persona 4", 48000)
        {
        }
    }
}