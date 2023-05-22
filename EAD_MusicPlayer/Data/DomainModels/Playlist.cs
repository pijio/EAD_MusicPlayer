using System.Collections.Generic;
using EAD_MusicPlayer.Areas.Songs.Pages.Songs;

namespace EAD_MusicPlayer.Data.DomainModels
{
    /// <summary>
    /// Доменная модель "Плейлист"
    /// </summary>
    public class Playlist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PathToCover { get; set; }
        
        public string OwnerId { get; set; }
        public User Owner { get; set; }
        
        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}