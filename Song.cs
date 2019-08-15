using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeetApp
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string AlbumName { get; set; }
        public TimeSpan Duration { get; set; }
        public string DurationStr { get{
                return String.Format("{0:D2}:{1:D2}",Duration.Minutes,Duration.Seconds);
            } }
        //public Picture CoverImage { get; set; }

    }
}
