// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Games
{
    using PhosLibrary.Builders.Music;

    /// <summary>
    /// Game music class for P4G.
    /// </summary>
    public class MusicP4G : GameMusicBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicP4G"/> class.
        /// </summary>
        public MusicP4G()
            : base(new BuilderP4G())
        {
        }
    }
}