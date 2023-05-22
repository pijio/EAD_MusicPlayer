using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAD_MusicPlayer.Data.DomainModels
{
    /// <summary>
    /// Доменная модель "Трек плейлиста"
    /// </summary>
    public class PlaylistSong
    {
        [Key]
        public string Id { get; set; }
        
        public string PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        public string SongId { get; set; }
        public Song Song { get; set; }
    }
}