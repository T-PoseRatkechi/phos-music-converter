// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Games
{
    using PhosLibrary.Builders.Music;

    /// <summary>
    /// Game music class for Persona 5.
    /// </summary>
    public class MusicP5 : GameMusicBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicP5"/> class.
        /// </summary>
        public MusicP5()
            : base(new BuilderP5())
        {
        }
    }
}