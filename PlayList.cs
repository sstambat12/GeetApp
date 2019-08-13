using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeetApp
{
    public class PlayList
    {
        private const string TEXT_FILE = "Playlist.txt";
        public List<Song> songs;

        public void WriteToFile()
        {
            foreach (var s in songs)
            {
                var content = s.Title + "," + s.Artist + "," + s.AlbumName + "," + s.Duration;
                FileHelper.WriteTextFileAsync(TEXT_FILE, content);
            }
        }

        public async static Task<PlayList> GetPlayListFromFileAsync()
        {
            var songs = new List<Song>();
            string content = await FileHelper.ReadTextFileAsync(TEXT_FILE);
            var lines = content.Split('\r', '\n');
            foreach(var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var lineParts = line.Split(',');
                var song = new Song
                {
                    Title = lineParts[0],
                    Artist = lineParts[1],
                    AlbumName = lineParts[2],
                    Duration = TimeSpan.Parse(lineParts[3])

                };
                songs.Add(song);
            }
            PlayList p = new PlayList();
            p.songs = songs;
            return p;
        }
        public void Add(Song song)
        {

        }

        public void Add(Album album)
        {

        }
    }
}
