using System.Collections.Generic;

namespace EAD_MusicPlayer.Data.DomainModels
{
    /// <summary>
    /// Доменная модель "Исполнитель"
    /// </summary>
    public class Author
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}