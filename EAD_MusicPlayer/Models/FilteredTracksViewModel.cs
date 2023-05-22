using System.Collections.Generic;

namespace EAD_MusicPlayer.Models
{
    public class FilteredTracksViewModel
    {
        public int PagesCount { get; set; }
        public ICollection<TrackViewModel> Tracks { get; set; }
    }
}