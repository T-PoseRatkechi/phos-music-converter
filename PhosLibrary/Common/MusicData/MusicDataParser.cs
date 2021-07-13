// Copyright (c) T-Pose Ratkechi. All rights reserved.
// Licensed under the GNU GPLv3 license. See LICENSE file in the project root for full license information.

namespace PhosLibrary.Common.MusicData
{
    using System.IO;
    using System.Text.Json;

    /// <summary>
    /// Parser class for Music Data JSON files.
    /// </summary>
    public class MusicDataParser
    {
        /// <summary>
        /// Parses adn returns the file at <paramref name="musicDataPath"/> as a <c>MusicData</c> object.
        /// </summary>
        /// <param name="musicDataPath">Path of file to parse.</param>
        /// <returns><paramref name="musicDataPath"/> parsed as <c>MusicData</c>.</returns>
        public static MusicData Parse(string musicDataPath)
        {
            string musicDataString = File.ReadAllText(musicDataPath);
            MusicData musicData = JsonSerializer.Deserialize<MusicData>(musicDataString);
            return musicData;
        }
    }
}