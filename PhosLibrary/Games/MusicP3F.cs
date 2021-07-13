// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Games
{
    using PhosLibrary.Builders.Music;

    /// <summary>
    /// Game music class for P3F and P4.
    /// </summary>
    public class MusicP3F : GameMusicBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicP3F"/> class.
        /// </summary>
        public MusicP3F()
            : base(new BuilderP3F())
        {
        }
    }
}