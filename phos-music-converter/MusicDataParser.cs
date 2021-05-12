using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace phos_music_converter
{
    struct MusicData
    {
        public Song[] songs { get; set; }
    }

    struct Song
    {
        public string id { get; set; }
        public bool isEnabled { get; set; }
        public string name { get; set; }
        public string originalFile { get; set; }
        public string replacementFilePath { get; set; }
        public int loopStartSample { get; set; }
        public int loopEndSample { get; set; }
        public string waveIndex { get; set; }
        public string uwuIndex { get; set; }
    }

    class MusicDataParser
    {
        public static MusicData ParseMusicData(string musicDataPath)
        {
            string musicDataString = File.ReadAllText(musicDataPath);
            MusicData musicData = JsonSerializer.Deserialize<MusicData>(musicDataString);
            return musicData;
        }
    }
}
