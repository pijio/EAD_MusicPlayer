using System.ComponentModel.DataAnnotations;

namespace EAD_MusicPlayer.Data.DomainModels
{
    /// <summary>
    /// Доменная модель "жанр"
    /// </summary>
    public class Genre
    {
        [Key]
        public string Id { get; set; }
        
        /// <summary>
        /// Название жанра
        /// </summary>
        public string Name { get; set; }
    }
}