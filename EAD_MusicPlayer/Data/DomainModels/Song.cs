using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EAD_MusicPlayer.Data.DomainModels
{
    /// <summary>
    /// Доменная модель "Трек"
    /// </summary>
    public class Song
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public string Id { get; set; }
        
        /// <summary>
        /// Имя трека
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Путь к файлу музыки
        /// </summary>
        public string PathToSong { get; set; }
        
        /// <summary>
        /// Путь к обложке
        /// </summary>
        public string PathToCover { get; set; }

        /// <summary>
        /// Дата загрузки или релиза
        /// </summary>
        public DateTime ReleaseDate { get; set; }
        
        /// <summary>
        /// Исполнитель (автор)
        /// </summary>
        public string AuthorId { get; set; }
        /// <summary>
        /// Исполнитель (автор)
        /// </summary>
        public Author Author { get; set; }

        /// <summary>
        /// Жанр
        /// </summary>
        public string GenreId { get; set; }
        /// <summary>
        /// Жанр
        /// </summary>
        public Genre Genre { get; set; }
        
        /// <summary>
        /// Треки в плейлистах
        /// </summary>
        public ICollection<PlaylistSong> PlaylistSongs { get; set; }
    }
}