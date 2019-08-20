using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeetApp
{
    public class Album
    {
        public string AlbumName { get; set; }
        public List<Song> Songs { get; set; }

        public void Add(Song song)
        {
            if(Songs == null)
            {
                Songs = new List<Song>();
            }
            Songs.Add(song);
        }

    }
}
