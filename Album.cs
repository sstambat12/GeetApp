using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace GeetApp
{
    public class Album
    {
        public string AlbumName { get; set; }
        public List<Song> Songs { get; set; }

        public BitmapImage CoverImage { get { return Songs[0].CoverImage; } }
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
