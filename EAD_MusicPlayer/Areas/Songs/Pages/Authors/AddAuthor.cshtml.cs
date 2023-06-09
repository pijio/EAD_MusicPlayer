﻿using System;
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

namespace EAD_MusicPlayer.Areas.Songs.Pages.Authors
{
    public class AddAuthor : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        
        [Display(Name = "Список исполнителей")]
        public ICollection<Author> Authors;

        public AddAuthor(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Authors = _dbContext.Authors.ToList();
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

        public async Task OnGetAsync()
        {
            Authors = await _dbContext.Authors.ToListAsync();
        }
        
        public async Task<IActionResult> OnPostDeleteAsync(string authorId)
        {
            var author = await _dbContext.Authors.FindAsync(authorId);

            if (author != null)
            {
                _dbContext.Authors.Remove(author);
                await _dbContext.SaveChangesAsync();
                Authors.Remove(author);
            }

            return RedirectToPage();
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
                    await _dbContext.SaveChangesAsync();
                    Authors.Add(author);
                    Input.AuthorName = string.Empty;
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