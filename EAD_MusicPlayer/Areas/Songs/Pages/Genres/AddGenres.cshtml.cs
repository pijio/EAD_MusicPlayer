using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Data.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EAD_MusicPlayer.Areas.Songs.Pages.Genres
{
    public class AddGenre : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        
        [Display(Name = "Список жанров")]
        public ICollection<Genre> Genres;

        public AddGenre(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Genres = _dbContext.Genres.ToList();
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Display(Name = "Название жанра")]
            [StringLength(maximumLength: 60, ErrorMessage = "Не более 60 символов", MinimumLength = 3)]
            [Required]
            public string GenreName { get; set; }
        }

        public async Task OnGetAsync()
        {
            Genres = await _dbContext.Genres.ToListAsync();
        }
        
        public async Task<IActionResult> OnPostDeleteAsync(string genreId)
        {
            var Genre = await _dbContext.Genres.FindAsync(genreId);

            if (Genre != null)
            {
                _dbContext.Genres.Remove(Genre);
                await _dbContext.SaveChangesAsync();
                Genres.Remove(Genre);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                if (_dbContext.Genres.FirstOrDefault(x => x.Name == Input.GenreName) != null)
                {
                    ModelState.AddModelError("Unique", "Жанр с таким именем уже существует!");
                    return Page();
                }

                try
                {
                    var Genre = new Genre { Id = Guid.NewGuid().ToString(), Name = Input.GenreName };
                    await _dbContext.Genres.AddAsync(Genre);
                    await _dbContext.SaveChangesAsync();
                    Genres.Add(Genre);
                    Input.GenreName = string.Empty;
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            return Page();
        }
    }
}