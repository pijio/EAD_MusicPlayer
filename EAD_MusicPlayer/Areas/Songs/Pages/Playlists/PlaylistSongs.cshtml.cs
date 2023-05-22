using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Data.DomainModels;
using EAD_MusicPlayer.Models;
using EAD_MusicPlayer.Services.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EAD_MusicPlayer.Areas.Songs.Pages.Playlists
{
    public class PlaylistSongs : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITrackService _trackService;
        private readonly SignInManager<User> _identityManager;
        private static int _pageSize = 10;

        [BindProperty(SupportsGet = true)]
        public PlaylistViewModel Playlist { get; private set; }
        
        [BindProperty(SupportsGet = true)]
        public IEnumerable<TrackViewModel> Tracks { get; private set; }
        
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PagesCount { get; set; }

        public PlaylistSongs(ApplicationDbContext dbContext, ITrackService trackService, SignInManager<User> identityManager)
        {
            _dbContext = dbContext;
            _trackService = trackService;
            _identityManager = identityManager;
        }

        public async Task<IActionResult> OnGetAsync(string playListId, int pageNo = 1)
        {
            Tracks = await _trackService.GetPlaylistTrack(playListId, pageNo, _pageSize);
            PagesCount = await _trackService.GetPagesCount(_pageSize);
            CurrentPage = pageNo;
            Playlist = await _dbContext.Playlists.Where(x => x.Id == playListId).Select(x => new PlaylistViewModel
                { Id = x.Id, Name = x.Name, PathToCover = x.PathToCover }).FirstOrDefaultAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteFromPlaylist(string trackId, string playlistId)
        {
            var song = _dbContext.PlaylistSongs.FirstOrDefault(x => x.SongId == trackId && x.PlaylistId == playlistId);
            if (song == null)
                return RedirectToPage();
            _dbContext.Remove(song);
            await _dbContext.SaveChangesAsync();
            return Page();
        }
    }
}