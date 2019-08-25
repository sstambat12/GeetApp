using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GeetApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        public PlayList currentPlayList;
        public Collection collection;
        public MainPageParams pageParams;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var myMusic = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Music);
            IObservableVector<StorageFolder> myMusicFolders = myMusic.Folders;
            collection = new Collection();
            foreach (var fold in myMusicFolders)
            {
                var query = fold.CreateFileQueryWithOptions(new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderByTitle, new string[] { ".mp3" }));
                var list = await query.GetFilesAsync();
                foreach (var file in list)
                {
                    Song song = await CreateSongFromFileAsync(file);
                    collection.Add(song);
                }
            }
            pageParams = new MainPageParams();
            pageParams.CentreFrame = CentreFrame;
            MediaHelper.MediaPlayer = MediaPlayer;
            //pageParams.NavigationControlView = NavigationControlView;
            pageParams.collection = collection;
            List<string> ListofPlaylistName = new List<string>();
            var storageFolder = ApplicationData.Current.LocalFolder;
            var files = await storageFolder.GetFilesAsync();
            foreach(var file in files)
            {
                ListofPlaylistName.Add(file.Name);
            }
            PlayList.UserPlayLists = ListofPlaylistName;
            PlayList.PlaylistAdded += PlaylistAdded;
            PlayList.PlaylistDeleted += PlaylistDeleted;
            foreach(var name in ListofPlaylistName)
            {
                Microsoft.UI.Xaml.Controls.NavigationViewItem viewItem = new Microsoft.UI.Xaml.Controls.NavigationViewItem();
                viewItem.Content = name;
                viewItem.Tapped += PlaylistSelected;
                NavigationControlView.MenuItems.Add(viewItem);
            }
            
            CentreFrame.Navigate(typeof(PivotDisplay), pageParams);

        }

        private async void PlaylistSelected(object sender, TappedRoutedEventArgs e)
        {
            string playlistName = (sender as Microsoft.UI.Xaml.Controls.NavigationViewItem).Content.ToString();
            PlayList p = await PlayList.GetPlayListFromFileAsync(playlistName);
            Tuple<PlayList, Frame> tuple = new Tuple<PlayList, Frame>(p, CentreFrame);
            CentreFrame.Navigate(typeof(PlaylistDisplay), tuple);
        }

        private async System.Threading.Tasks.Task<Song> CreateSongFromFileAsync(StorageFile file)
        {
            if (file != null)
            {
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                BitmapImage image = await GetThumbnail(file);

                Song song = new Song
                {
                    Title = musicProperties.Title,
                    Artist = musicProperties.Artist,
                    Duration = musicProperties.Duration,
                    AlbumName = musicProperties.Album,
                    Path = file.Path,
                    FileName=file.Name,
                    CoverImage=image
            };
                if (string.IsNullOrEmpty(song.Title))
                {
                    song.Title = file.DisplayName;
                }
                if (string.IsNullOrEmpty(song.Artist))
                {
                    song.Artist = "Unknown Artist";
                }
                if (string.IsNullOrEmpty(song.AlbumName))
                {
                    song.AlbumName = "Unknown Album";
                }
                return song;
            }
            return null;
        }
        private async static Task<BitmapImage> GetThumbnail(StorageFile file)
        {
            if (file != null)
            {
                StorageItemThumbnail thumb = await file.GetScaledImageAsThumbnailAsync(ThumbnailMode.MusicView, 200, ThumbnailOptions.ResizeThumbnail);
                if (thumb != null)
                {
                    BitmapImage img = new BitmapImage();
                    await img.SetSourceAsync(thumb);
                    return img;
                }
            }
            return null;
        }



        private void NavigationControlView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            CentreFrame.GoBack();
        }

        private void MyMusicMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CentreFrame.Navigate(typeof(PivotDisplay), pageParams);
        }

        private void PlaylistDeleted(object sender, EventArgs e)
        {
            int index = (int)sender;
            NavigationControlView.MenuItems.RemoveAt(index + 3);
        }
        private void PlaylistAdded(object sender, EventArgs e)
        {
            string playlistName = (string)sender;
            Microsoft.UI.Xaml.Controls.NavigationViewItem viewItem = new Microsoft.UI.Xaml.Controls.NavigationViewItem();
            viewItem.Content = playlistName;
            viewItem.Tapped += PlaylistSelected;
            NavigationControlView.MenuItems.Add(viewItem);
        }
    }
}
        