using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GeetApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistDisplay : Page
    {
        Frame CentreFrame;
        public PlaylistDisplay()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Tuple<PlayList, Frame> tuple = e.Parameter as Tuple<PlayList, Frame>;
            PlayList playList = tuple.Item1;
            CentreFrame = tuple.Item2;
            listOfSongs.DataContext = playList.songs;
            Title.Text = playList.Name;
            if(playList.songs.Count>0)
            {
               // PlaylistImage.Source = playList.songs[0].CoverImage;
            }
            
                                                                                                        

        }
        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject iterator = sender as DependencyObject;

            while (!(iterator is ListViewItem))
            {
                iterator = VisualTreeHelper.GetParent(iterator);

            }
            DependencyObject parent = VisualTreeHelper.GetParent(iterator);
            Panel panel = parent as Panel;
            int index = panel.Children.IndexOf(iterator as UIElement);
            List<Song> list = listOfSongs.DataContext as List<Song>;
            List<Song> songToPlay = new List<Song>();
            songToPlay.Add(list[index]);
            MediaPlaybackList playbackList = await MediaHelper.GetPlaybackList(songToPlay);
            MediaHelper.MediaPlayer.Source = playbackList;
        }

        private async void PlayAll_Click(object sender, RoutedEventArgs e)
        {
            List<Song> list = listOfSongs.DataContext as List<Song>;
            MediaPlaybackList playbacklist = await MediaHelper.GetPlaybackList(list);
            playbacklist.ShuffleEnabled = true;
            MediaHelper.MediaPlayer.Source = playbacklist;
        }

        private void DeletePlaylist_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = Title.Text;
            FileHelper.DeleteFile(playlistName);
            int index = PlayList.UserPlayLists.IndexOf(playlistName);
            PlayList.UserPlayLists.Remove(playlistName);
            PlayList.PlaylistDeleted.Invoke(index, new EventArgs());
            CentreFrame.GoBack();
        }

        private void DeleteSong_Click(object sender, RoutedEventArgs e)
        {
            string playlistName = Title.Text;
            List<Song> PlaylistSongs = new List<Song>();
            var Indexes = listOfSongs.SelectedRanges;
            List<Song> Mainlist = listOfSongs.DataContext as List<Song>;
            var reversedIndexes = Indexes.Reverse();
            foreach (var index in reversedIndexes)
            {
                int firstindex = index.FirstIndex;
                int lastindex = index.LastIndex;
                Mainlist.RemoveRange(firstindex, (int)index.Length);
                
            }
            PlayList playList = new PlayList();
            playList.Name = playlistName;
            playList.songs = Mainlist;
            playList.WriteToFileAsync();
            listOfSongs.DataContext = null;
            listOfSongs.DataContext = Mainlist;
        }
    }
}
