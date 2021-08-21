// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Common.MusicData
{
#pragma warning disable SA1300, IDE1006

    /// <summary>
    /// Music Data object struct.
    /// </summary>
    public struct MusicData
    {
        /// <summary>
        /// Gets the list of <c>Song</c>s of <c>MusicData</c>.
        /// </summary>
        public Song[] songs { get; set; }
    }

    /// <summary>
    /// Song object struct.
    /// </summary>
    public struct Song
    {
        /// <summary>
        /// Gets song ID.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets a value indicating whether the song is enabled for replacing.
        /// </summary>
        public bool isEnabled { get; set; }

        /// <summary>
        /// Gets song name.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Gets original file name.
        /// </summary>
        public string originalFile { get; set; }

        /// <summary>
        /// Gets path to the replacement file.
        /// </summary>
        public string replacementFilePath { get; set; }

        /// <summary>
        /// Gets loop start sample.
        /// </summary>
        public int loopStartSample { get; set; }

        /// <summary>
        /// Gets loop end sample.
        /// </summary>
        public int loopEndSample { get; set; }

        /// <summary>
        /// Gets the replacement output path of song.
        /// </summary>
        public string outputFilePath { get; set; }

        /// <summary>
        /// Gets extra data for song.
        /// </summary>
        public string extraData { get; set; }
    }
}