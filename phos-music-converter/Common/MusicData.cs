// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosMusicConverter.Common
{
#pragma warning disable SA1300, IDE1006

    /// <summary>
    /// Music Data object struct.
    /// </summary>
    internal struct MusicData
    {
        /// <summary>
        /// Gets the list of <c>Song</c>s of <c>MusicData</c>.
        /// </summary>
        public Song[] songs { get; init; }
    }

    /// <summary>
    /// Song object struct.
    /// </summary>
    internal struct Song
    {
        /// <summary>
        /// Gets song ID.
        /// </summary>
        public string id { get; init; }

        /// <summary>
        /// Gets a value indicating whether the song is enabled for replacing.
        /// </summary>
        public bool isEnabled { get; init; }

        /// <summary>
        /// Gets song name.
        /// </summary>
        public string name { get; init; }

        /// <summary>
        /// Gets original file name.
        /// </summary>
        public string originalFile { get; init; }

        /// <summary>
        /// Gets path to the replacement file.
        /// </summary>
        public string replacementFilePath { get; init; }

        /// <summary>
        /// Gets loop start sample.
        /// </summary>
        public int loopStartSample { get; init; }

        /// <summary>
        /// Gets loop end sample.
        /// </summary>
        public int loopEndSample { get; init; }

        /// <summary>
        /// Gets the replacement output path of song.
        /// </summary>
        public string outputFilePath { get; init; }

        /// <summary>
        /// Gets extra data for song.
        /// </summary>
        public string extraData { get; init; }
    }
}