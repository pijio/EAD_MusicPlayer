using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Data.DomainModels;
using EAD_MusicPlayer.Helpers;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EAD_MusicPlayer.Areas.Songs.Pages.Songs
{
    public class AddSong : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public AddSong(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
            GetSelectedLists();
        }

        public void GetSelectedLists()
        {
            Genres = _dbContext.Genres.ToList();
            Authors = _dbContext.Authors.ToList();
        }

        public async Task<IActionResult> OnPostAsync([FromServices] IWebHostEnvironment env)
        {
            if (!ModelState.IsValid) return Page();
            if (!Input.Cover.ContentType.StartsWith("image/") || Input.Cover == null)
            {
                ModelState.AddModelError("NotImage", "Для обложки трека нужно изображение любого расширения");
                return Page();
            }
            if (!Input.Track.ContentType.StartsWith("audio/") || Input.Track == null)
            {
                ModelState.AddModelError("NotAudio", "Загруженный файл трека не является аудиофайлом");
                return Page();
            }

            var trackId = Guid.NewGuid().ToString();
            var pathToTrack = Path.Combine("tracks", trackId + Path.GetExtension(Input.Track.FileName).ToLower());
            await FileHelper.SaveFile(Input.Track, Path.Combine(env.WebRootPath, pathToTrack));
            var coverId = Guid.NewGuid().ToString();
            var pathToCover = Path.Combine("covers", coverId + Path.GetExtension(Input.Cover.FileName).ToLower());
            await FileHelper.SaveFile(Input.Cover, Path.Combine(env.WebRootPath, pathToCover));

            if (_dbContext.Songs.FirstOrDefault(x => x.Name == Input.SongName) != null)
            {
                ModelState.AddModelError("NotAudio", "Трек с таким именем уже существует");
                return Page();
            }

            var song = new Song()
            {
                AuthorId = Input.AuthorId,
                GenreId = Input.GenreId,
                Id = trackId,
                Name = Input.SongName,
                PathToCover = pathToCover,
                PathToSong = pathToTrack,
                ReleaseDate = Input.ReleaseDate
            };

            await _dbContext.Songs.AddAsync(song);
            await _dbContext.SaveChangesAsync();
            Input = new FormModel();
            return Page();
        }

        /// <summary>
        /// Доступные жанры
        /// </summary>
        public ICollection<Genre> Genres { get; private set; }

        /// <summary>
        /// Доступные исполнители
        /// </summary>
        public ICollection<Author> Authors { get; private set; }
        
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
            [Display(Name = "Название песни")]
            [Required]
            public string SongName { get; set; }
            
            /// <summary>
            /// Автор трека
            /// </summary>
            [Display(Name = "Исполнитель")]
            [Required]
            public string AuthorId { get; set; }
            
            /// <summary>
            /// Жанр трека
            /// </summary>
            [Display(Name = "Жанр")]
            [Required]
            public string GenreId { get; set; }

            /// <summary>
            /// Дата релиза трека (null если совпадает с датой загрузки)
            /// </summary>
            [Display(Name = "Дата релиза трека")]
            public DateTime ReleaseDate { get; set; } = DateTime.Now;

            /// <summary>
            /// Файл трека
            /// </summary>
            [Display(Name = "Файл трека")]
            [Required]
            public IFormFile Track { get; set; }
            
            /// <summary>
            /// Обложка трека
            /// </summary>
            [Display(Name = "Обложка")]
            [Required]
            public IFormFile Cover { get; set; }
        }
    }
}