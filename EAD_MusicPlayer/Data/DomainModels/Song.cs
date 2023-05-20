﻿using System;

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
        public Author Author { get; set; }
    }
}