using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Models;
using EAD_MusicPlayer.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EAD_MusicPlayer.Areas.Songs.Pages.Songs
{
    [Area("Songs")]
    [Route("/Songs/Songs")]
    public class Songs : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITrackService _trackService;
        private static int _pageSize = 10;
        
        [BindProperty(SupportsGet = true)]
        public IEnumerable<TrackViewModel> Tracks { get; private set; }
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PagesCount { get; set; }
        
        public Songs(ApplicationDbContext dbContext, ITrackService trackService)
        {
            _dbContext = dbContext;
            _trackService = trackService;
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
        
        [Authorize]
        public async Task<IActionResult> OnGetAsync(int pageNo = 1)
        {
            Tracks = await _trackService.GetTracksPage(pageNo, _pageSize);
            PagesCount = await _trackService.GetPagesCount(_pageSize);
            CurrentPage = pageNo;
            return Page();
        }
    }
}