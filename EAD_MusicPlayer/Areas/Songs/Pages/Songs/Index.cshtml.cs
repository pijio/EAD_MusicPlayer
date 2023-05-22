using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Data.DomainModels;
using EAD_MusicPlayer.Models;
using EAD_MusicPlayer.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EAD_MusicPlayer.Areas.Songs.Pages.Songs
{
    [Area("Songs")]
    public class Songs : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITrackService _trackService;
        private readonly SignInManager<User> _identityManager;
        private static int _pageSize = 10;
        
        public  ICollection<PlaylistViewModel> Playlist { get; private set; }
        
        [BindProperty(SupportsGet = true)]
        public IEnumerable<TrackViewModel> Tracks { get; private set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PagesCount { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public SearchModel SearchPattern { get; set; }

        public Songs(ApplicationDbContext dbContext, ITrackService trackService, SignInManager<User> identityManager)
        {
            _dbContext = dbContext;
            _trackService = trackService;
            _identityManager = identityManager;
        }
        
        private async Task<IEnumerable<PlaylistViewModel>> GetPlaylists()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _identityManager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("User", "Непридвиденная ошибка");
                return null;
            }

            if (await _identityManager.UserManager.IsInRoleAsync(user, "Admin"))
            {
                return await _trackService.GetUserPlaylists();
            }
            return await _trackService.GetUserPlaylists(userId);
        }
        
        public async Task<IActionResult> OnPostDeleteAsync(string trackId)
        {
            var track = await _dbContext.Songs.FindAsync(trackId);

            if (track == null) return RedirectToPage();
            _dbContext.Songs.Remove(track);
            await _dbContext.SaveChangesAsync();
            Tracks = await _trackService.GetTracksPage(CurrentPage, _pageSize);
            return RedirectToPage();
        }
        
        public async Task<IActionResult> OnGetAsync(int pageNo = 1)
        {
            Tracks = await _trackService.GetTracksPage(pageNo, _pageSize);
            PagesCount = await _trackService.GetPagesCount(_pageSize);
            CurrentPage = pageNo;
            Playlist = (await GetPlaylists()).ToList();
            return Page();
        }
        
        public async Task<IActionResult> OnPostSearch()
        {
            if (string.IsNullOrEmpty(SearchPattern.SearchText))
                return RedirectToPage();
            Tracks = (await _trackService.GetFilteredTracks(SearchPattern)).Tracks;
            PagesCount = 0;
            CurrentPage = 1;
            Playlist = (await GetPlaylists()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAddTrackToPlaylist(string trackId, string playlistId)
        {
            var playListTrackId = Guid.NewGuid().ToString();
            if (_dbContext.PlaylistSongs.FirstOrDefault(x => x.PlaylistId == playlistId && x.SongId == trackId) != null)
                return RedirectToPage();
            var playlistSong = new PlaylistSong() { Id = playListTrackId, PlaylistId = playlistId, SongId = trackId };
            await _dbContext.AddAsync(playlistSong);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage();
        }
        
        public class SearchModel
        {
            [Display(Name = "Поиск")]
            public string SearchText { get; set; }
            public bool FindBySongName { get; set; }
            public bool FindByAuthorName { get; set; }
        }
    }
}