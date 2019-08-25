using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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
    public sealed partial class DisplaySongDetails : Page
    {
        private Song song;
        public DisplaySongDetails()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var song = (Song)e.Parameter;
            this.song = song;
            title.DataContext = song.Title;
            album.DataContext = song.AlbumName;
            artist.DataContext = song.Artist;
            
            CoverImage.DataContext = song.CoverImage;
        }

        private void save_title(object sender, TextChangedEventArgs args)
        {
            this.song.Title = title.Text;
        }

        private void save_album(object sender, TextChangedEventArgs args)
        {
            this.song.AlbumName = album.Text;

        }
        private void save_artist(object sender, TextChangedEventArgs args)
        {
            this.song.Artist = artist.Text;
        }

        private async void ChangeImage_DoubleTapped(object sender, DoubleTappedRoutedEventArgs args)
        {
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            StorageFile file = await filePicker.PickSingleFileAsync();
            song.CoverImage = await StorageFileToBitmapImage(file);
            CoverImage.DataContext = song.CoverImage;
        }

        private  async Task<BitmapImage> StorageFileToBitmapImage(StorageFile savedStorageFile)
        {
            using (IRandomAccessStream fileStream = await savedStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                return bitmapImage;
            }
        }

        private async void SaveSong_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder musicFolder = KnownFolders.MusicLibrary;
            StorageFile musicFile = await musicFolder.GetFileAsync(song.FileName);
            var musicProperties = await musicFile.Properties.GetMusicPropertiesAsync();
            musicProperties.Title = song.Title;
            musicProperties.Artist = song.Artist;
            musicProperties.Album = song.AlbumName;
            await musicProperties.SavePropertiesAsync();
            StorageItemThumbnail thumb = await musicFile.GetScaledImageAsThumbnailAsync(ThumbnailMode.PicturesView);
        }

        
    }



}

