using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PlayList playlist = await PlayList.GetPlayListFromFileAsync();
            this.currentPlayList = playlist;
            this.DataContext = currentPlayList.songs;
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
                    AlbumName = musicProperties.Album
                };
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
            this.DataContext = currentPlayList.songs;
            
            PlayList p = new PlayList();
            p.songs = listOfSongs;
            p.WriteToFile();
        }

        private void DeleteSong_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            int row = SongsGrid.Children.IndexOf(btn.Parent as StackPanel);
            
        }

    }
}
