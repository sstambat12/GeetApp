using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeetApp
{
    public class PlayList
    {
       // private const string TEXT_FILE = "Playlist.txt";
        public List<Song> songs;
        public string Name { get; set; }

        public static List<string> UserPlayLists { get; set; }

        public static EventHandler PlaylistDeleted { get; set; }
        public static EventHandler PlaylistAdded { get; set; }

        public async void WriteToFileAsync()
        {
            string content = "";
            foreach (var s in songs)
            {
                content = content + "\n" + s.Title + "," + s.Artist + "," + s.AlbumName + "," + s.Duration+ ","+ s.Path ;
                
            }
            await FileHelper.WriteTextFileAsync(Name, content);
        }

        public async static Task<PlayList> GetPlayListFromFileAsync(string fileName)
        {
            var songs = new List<Song>();
            string content = await FileHelper.ReadTextFileAsync(fileName);
            var lines = content.Split('\r','\n');
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
                    Duration = TimeSpan.Parse(lineParts[3]),
                    Path = lineParts[4]

                };
                songs.Add(song);
            }
            PlayList p = new PlayList();
            p.Name = fileName;
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
