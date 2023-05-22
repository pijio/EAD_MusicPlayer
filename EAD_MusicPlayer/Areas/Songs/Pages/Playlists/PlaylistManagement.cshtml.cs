using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Data.DomainModels;
using EAD_MusicPlayer.Helpers;
using EAD_MusicPlayer.Models;
using EAD_MusicPlayer.Services.Base;
using EAD_MusicPlayer.Services.Implementation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EAD_MusicPlayer.Areas.Songs.Pages.Playlists
{
    public class AddPlaylist : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITrackService _trackService;
        private readonly SignInManager<User> _identityManager;

        [BindProperty]
        public ICollection<PlaylistViewModel> Playlists { get; private set; }
        
        public AddPlaylist(ApplicationDbContext applicationDbContext, ITrackService trackService, SignInManager<User> identityManager)
        {
            _dbContext = applicationDbContext;
            _trackService = trackService;
            _identityManager = identityManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Playlists = (await GetPlaylists()).ToList();
            return Page();
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
        
        public async Task<IActionResult> OnPostAsync([FromServices] IWebHostEnvironment env, [FromServices] SignInManager<User> manager)
        {
            if (!ModelState.IsValid) return Page();
            if (!Input.Cover.ContentType.StartsWith("image/") || Input.Cover == null)
            {
                ModelState.AddModelError("NotImage", "Для обложки трека нужно изображение любого расширения");
                return RedirectToPage();
            }
            
            var playlistId = Guid.NewGuid().ToString();
            var pathToCover = Path.Combine("playlists", playlistId + Path.GetExtension(Input.Cover.FileName).ToLower());
            await FileHelper.SaveFile(Input.Cover, Path.Combine(env.WebRootPath, pathToCover));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await manager.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("User", "Непридвиденная ошибка");
                return RedirectToPage();
            }
            if (_dbContext.Playlists.FirstOrDefault(x => x.Name == Input.PlaylistName && user.Id == x.OwnerId) != null)
            {
                ModelState.AddModelError("Exists", "Плейлист с таким именем уже существует");
                return RedirectToPage();
            }

            var playlist = new Playlist()
            {
                Id = playlistId,
                Name = Input.PlaylistName,
                PathToCover = pathToCover,
                OwnerId = user.Id
            };

            await _dbContext.Playlists.AddAsync(playlist);
            await _dbContext.SaveChangesAsync();
            Input = new FormModel();
            Playlists = (await GetPlaylists()).ToList();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string playlistId)
        {
            var playlist = await _dbContext.Playlists.FirstOrDefaultAsync(x => x.Id == playlistId);
            if (playlist == null) return RedirectToPage();
            _dbContext.Playlists.Remove(playlist);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage();
        }

        /// <summary>
        /// Ввод с формы
        /// </summary>
        [BindProperty]
        public FormModel Input { get; set; }

        public class FormModel
        {
            /// <summary>
            /// Название трека
            /// </summary>
            [Display(Name = "Название плейлиста")]
            [Required]
            public string PlaylistName { get; set; }

            /// <summary>
            /// Обложка трека
            /// </summary>
            [Display(Name = "Обложка")]
            [Required]
            public IFormFile Cover { get; set; }
        }
    }
}