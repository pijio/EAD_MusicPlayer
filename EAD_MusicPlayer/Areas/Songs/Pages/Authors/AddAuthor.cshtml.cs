using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Data.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EAD_MusicPlayer.Areas.Songs.Pages.Authors
{
    public class AddAuthor : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        
        public AddAuthor(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Display(Name = "Имя автора")]
            [StringLength(maximumLength: 60, ErrorMessage = "Не более 60 символов", MinimumLength = 3)]
            [Required]
            public string AuthorName { get; set; }
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                if (_dbContext.Authors.FirstOrDefault(x => x.Name == Input.AuthorName) != null)
                {
                    ModelState.AddModelError("Unique", "Автор с таким именем уже существует!");
                    return Page();
                }

                try
                {
                    var author = new Author { Id = Guid.NewGuid().ToString(), Name = Input.AuthorName };
                    await _dbContext.Authors.AddAsync(author);
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