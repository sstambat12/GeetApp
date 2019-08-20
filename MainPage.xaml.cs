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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var myMusic = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Music);
            IObservableVector<StorageFolder> myMusicFolders = myMusic.Folders;
            collection = new Collection();
            foreach(var fold in myMusicFolders)
            {
                var query = fold.CreateFileQueryWithOptions(new Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderByTitle, new string[] { ".mp3" }));
                var list = await query.GetFilesAsync();
                foreach(var file in list)
                {
                    Song song = await CreateSongFromFileAsync(file);
                    collection.Add(song);
                }
            }
            listOfSongs.DataContext = collection.GetListofSongs();
        }
        private async System.Threading.Tasks.Task<Song> CreateSongFromFileAsync(StorageFile file)
        {
            if(file != null)
            {
                MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                Song song = new Song
                {
                    Title = musicProperties.Title,
                    Artist = musicProperties.Artist,
                    Duration = musicProperties.Duration,
                    AlbumName = musicProperties.Album,
                    Path = file.Path                    
                };
                if(song.Title.Length == 0)
                {
                    song.Title = file.DisplayName;
                }
                if(song.Artist.Length == 0)
                {
                    song.Artist = "Unknown Artist";
                }
                if(song.AlbumName.Length == 0)
                {
                    song.AlbumName = "Unknown Album";
                }
                return song;
            }
            return null;
        }

        private async void AddSong_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            

            IReadOnlyList<StorageFile> fileList = await openPicker.PickMultipleFilesAsync();
            List<Song> listOfSongs = new List<Song>();
            
            foreach(var file in fileList)
            {
                Song song = await CreateSongFromFileAsync(file);
                listOfSongs.Add(song);
            }
            this.currentPlayList.songs.AddRange(listOfSongs);
            this.DataContext = null;
            this.DataContext = currentPlayList.songs;
            
            PlayList p = new PlayList();
            p.songs = listOfSongs;
            p.WriteToFileAsync();
        }

        private async void DeleteSong_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject iterator = sender as DependencyObject;

            while(!(iterator is ListViewItem))
            {
                iterator = VisualTreeHelper.GetParent(iterator);

            }
            DependencyObject parent = VisualTreeHelper.GetParent(iterator);
            Panel panel = parent as Panel;
            int index = panel.Children.IndexOf(iterator as UIElement);
            StorageFile file = await StorageFile.GetFileFromPathAsync(currentPlayList.songs[index].Path);

            var songStream = await file.OpenAsync(FileAccessMode.Read);
            MediaPlayer.SetSource(songStream, "audio/mpeg");
            MediaPlaybackList playbacklist = new MediaPlaybackList();
            MediaPlayer.Play();
                       
        }

        private async void SelectPhoto_ClickAsync(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            StorageFile file = await openPicker.PickSingleFileAsync();
            var imageStream= await file.OpenAsync(FileAccessMode.Read);
            string path = file.Path;
            BitmapImage bitMap = new BitmapImage();
            await bitMap.SetSourceAsync(imageStream);
            Cover.Source = bitMap;

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivot = null;
            pivot = (PivotItem)(sender as Pivot).SelectedItem;
            switch (pivot.Header.ToString())
            {
                case "Songs":
                    if(collection != null)
                        listOfSongs.DataContext = collection.GetListofSongs();
                    break;
                case "Albums":
                    if(collection != null)
                        AlbumGrid.DataContext = collection.GetListofAlbums();
                    break;
            }
        }
    }
}
