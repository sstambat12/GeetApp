using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace GeetApp
{
    class MediaHelper
    {
        public static MediaPlayerElement MediaPlayer { get; set; }
        public async static Task<MediaPlaybackList> GetPlaybackList(List<Song> listOfSongs)
        {
            MediaPlaybackList playbacklist = new MediaPlaybackList();
            playbacklist.AutoRepeatEnabled = true;
            foreach (var song in listOfSongs)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(song.Path);
                MediaSource source = MediaSource.CreateFromStorageFile(file);
                playbacklist.Items.Add(new MediaPlaybackItem(source));
            }
            return playbacklist;

        }
    }
}
