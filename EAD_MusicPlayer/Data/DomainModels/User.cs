using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EAD_MusicPlayer.Data.DomainModels
{
    public class User : IdentityUser
    {
        public ICollection<Playlist> Playlists { get; set; }
    }
}