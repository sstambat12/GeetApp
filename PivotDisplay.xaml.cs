using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GeetApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PivotDisplay : Page
    {
        public PivotDisplay()
        {
            this.InitializeComponent();
        }

        public Collection collection;
        MainPageParams mainPageParams;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            mainPageParams = e.Parameter as MainPageParams;
            collection = mainPageParams.collection;
            listOfSongs.DataContext = collection.GetListofSongs();

            
            List<string> PlaylistNames = PlayList.UserPlayLists;
            foreach(var name in PlaylistNames)
            {
                MenuFlyoutItem menuflyoutitem = new MenuFlyoutItem();
                menuflyoutitem.Text = name;
                menuflyoutitem.Click += PlaylistName_Click;
                AddToMenuFlyout.Items.Add(menuflyoutitem);
            }
                     
        }


        private void AddTo_Click(object sender, RoutedEventArgs e)
        {
           FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void PlaySong_Click(object sender, RoutedEventArgs e)
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

        private async void EditSong_Click(object sender, RoutedEventArgs e)
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

            this.Frame.Navigate(typeof(DisplaySongDetails), list[index]);
        }


            private async void SelectPhoto_ClickAsync(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            StorageFile file = await openPicker.PickSingleFileAsync();
            var imageStream = await file.OpenAsync(FileAccessMode.Read);
            string path = file.Path;
            BitmapImage bitMap = new BitmapImage();
            await bitMap.SetSourceAsync(imageStream);
            //Cover.Source = bitMap;

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivot = null;
            pivot = (PivotItem)(sender as Pivot).SelectedItem;
            switch (pivot.Header.ToString())
            {
                case "Songs":
                    if (collection != null)
                        listOfSongs.DataContext = collection.GetListofSongs();
                    break;
                case "Albums":
                    if (collection != null)
                        AlbumGrid.DataContext = collection.GetListofAlbums();
                    break;
            }
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
           // NavigationControlView.IsBackEnabled = true;
            StackPanel stackPanel = sender as StackPanel;
            string albumName = (stackPanel.Children.ElementAt(1) as TextBlock).Text;
            List<Song> songs = collection.GetListofSongs(albumName);
            
            mainPageParams.CentreFrame.Navigate(typeof(AlbumPage), collection.GetAlbum(albumName));
        }

        private async void PlayAll_Click(object sender, RoutedEventArgs e)
        {
            List<Song> list = listOfSongs.DataContext as List<Song>;
            MediaPlaybackList playbacklist = await MediaHelper.GetPlaybackList(list);
            playbacklist.ShuffleEnabled = true;
            MediaHelper.MediaPlayer.Source = playbacklist;

        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            listOfSongs.SelectAll();
        }

        private void CancelAll_Click(object sender, RoutedEventArgs e)
        {
            listOfSongs.DeselectRange(new ItemIndexRange(0, (uint)listOfSongs.Items.Count));
        }

        private async void NewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylistDialog dialog = new CreatePlaylistDialog();
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Secondary)
            {
                return;
            }
            string playlistName = dialog.PlaylistName;
            List<Song> PlaylistSongs = new List<Song>();
            var Indexes = listOfSongs.SelectedRanges;
            List<Song> Mainlist = listOfSongs.DataContext as List<Song>;

            foreach(var index in Indexes)
            {
                int firstindex = index.FirstIndex;
                int lastindex = index.LastIndex;
                PlaylistSongs.AddRange(Mainlist.GetRange(firstindex, (int)index.Length));
            }
            PlayList playList = new PlayList();
            playList.Name = playlistName;
            playList.songs = PlaylistSongs;
            playList.WriteToFileAsync();
            PlayList.UserPlayLists.Add(playlistName);
            PlayList.PlaylistAdded.Invoke(playlistName, new EventArgs());
            Tuple<PlayList, Frame> tuple = new Tuple<PlayList, Frame>(playList, mainPageParams.CentreFrame);
            mainPageParams.CentreFrame.Navigate(typeof(PlaylistDisplay), tuple);
        }

        private async void PlaylistName_Click(object sender, RoutedEventArgs e)
        {
            // Get user selected songs
            List<Song> UserSelectedSongs = new List<Song>();
            var Indexes = listOfSongs.SelectedRanges;
            List<Song> Mainlist = listOfSongs.DataContext as List<Song>;

            foreach (var index in Indexes)
            {
                int firstindex = index.FirstIndex;
                int lastindex = index.LastIndex;
                UserSelectedSongs.AddRange(Mainlist.GetRange(firstindex, (int)index.Length));
            }

            // Get playlist name
            MenuFlyoutItem flyoutItem = sender as MenuFlyoutItem;
            string PlaylistName = flyoutItem.Text;

            // Create Playlist object : playlist name in which we have existing songs
            PlayList playList= await PlayList.GetPlayListFromFileAsync(PlaylistName);

            // Add selected songs in playlist
            playList.songs.AddRange(UserSelectedSongs);

            // Display Playlist object in PlaylistDisplay Page
            playList.WriteToFileAsync();
            Tuple<PlayList, Frame> tuple = new Tuple<PlayList, Frame>(playList, mainPageParams.CentreFrame);
            mainPageParams.CentreFrame.Navigate(typeof(PlaylistDisplay), tuple);
        }
    }
}
