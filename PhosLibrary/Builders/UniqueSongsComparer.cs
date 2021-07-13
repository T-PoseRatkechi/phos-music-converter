// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Builders
{
    using System.Collections.Generic;
    using PhosLibrary.Common.MusicData;

    /// <summary>
    /// Comparer class for generating list of unique songs to be encoded.
    /// </summary>
    internal class UniqueSongsComparer : IEqualityComparer<Song>
    {
        /// <inheritdoc/>
        public bool Equals(Song x, Song y)
        {
            return x.replacementFilePath.Equals(y.replacementFilePath);
        }

        /// <inheritdoc/>
        public int GetHashCode(Song obj)
        {
            return obj.replacementFilePath.GetHashCode();
        }
    }
}