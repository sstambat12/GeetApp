using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeetApp
{
    public class Collection
    {
        Dictionary<string, Album> AlbumsDictionary = new Dictionary<string, Album>();
        public void Add(Song song)
        {
            if(AlbumsDictionary.ContainsKey(song.AlbumName))
            {
                AlbumsDictionary[song.AlbumName].Add(song);
            }
            else
            {
                Album album = new Album();
                album.AlbumName = song.AlbumName;
                album.Add(song);
                AlbumsDictionary.Add(song.AlbumName, album);
                
            }
        }

        public List<Song> GetListofSongs()
        {
            List<Song> songs = new List<Song>();
            foreach(var album in AlbumsDictionary.Values)
            {
                songs.AddRange(album.Songs);
                    
            }
            return songs;
        }

        public ICollection<Album> GetListofAlbums()
        {
            return AlbumsDictionary.Values;
        }

        public List<Song> GetListofSongs(string albumName)
        {
            if(AlbumsDictionary.ContainsKey(albumName))
            {
                return AlbumsDictionary[albumName].Songs;
            }
            return null;
        }

        public Album GetAlbum(string albumName)
        {
            return AlbumsDictionary[albumName];
        }
    }
}
