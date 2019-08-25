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
    public sealed partial class AlbumPage : Page
    {
        public AlbumPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Album album = e.Parameter as Album;
            AlbumImage.Source = album.CoverImage;
            Title.Text = album.AlbumName;
            listOfSongs.DataContext = album.Songs;
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
    }
}
