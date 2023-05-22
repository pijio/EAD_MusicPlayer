using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EAD_MusicPlayer.Areas.Songs.Pages.Songs;
using EAD_MusicPlayer.Data;
using EAD_MusicPlayer.Data.DomainModels;
using EAD_MusicPlayer.Data.Migrations;
using EAD_MusicPlayer.Models;
using EAD_MusicPlayer.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace EAD_MusicPlayer.Services.Implementation
{
    /// <summary>
    /// Сервис по работе с треками
    /// </summary>
    public class TrackService : ITrackService
    {
        private readonly ApplicationDbContext _dbContext;
        public TrackService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<List<TrackViewModel>> GetAllTracks()
        {
            var all =  await _dbContext.Songs.Include(x => x.Author).Select(x =>
                new TrackViewModel
                {
                    AuthorName = x.Author.Name,
                    Id = x.Id,
                    Name = x.Name,
                    PathToCover = x.PathToCover,
                    PathToTrack = x.PathToSong
                }).ToListAsync();
            return all;
        }

        public async Task<List<TrackViewModel>> GetTracksPage(int pageNo, int pageSize)
        {
            var page = await _dbContext.Songs.Include(x => x.Author).Skip((pageNo - 1) * pageSize).Take(pageSize).Select(x =>
                new TrackViewModel
                {
                    AuthorName = x.Author.Name,
                    Id = x.Id,
                    Name = x.Name,
                    PathToCover = x.PathToCover,
                    PathToTrack = x.PathToSong
                }).ToListAsync();
            return page;
        }

        public async Task<List<TrackViewModel>> GetPlaylistTrack(string playlistId, int pageNo, int pageSize)
        {
            var page = await _dbContext.PlaylistSongs.Include(x => x.Song).Where(x => x.PlaylistId == playlistId).Skip((pageNo - 1) * pageSize).Take(pageSize).Select(x =>
                new TrackViewModel
                {
                    AuthorName = x.Song.Author.Name,
                    Id = x.Id,
                    Name = x.Song.Name,
                    PathToCover = x.Song.PathToCover,
                    PathToTrack = x.Song.PathToSong
                }).ToListAsync();
            return page;
        }

        public async Task<int> GetPagesCount(int pageSize)
        {
            var tracksCount = await _dbContext.Songs.CountAsync();
            if (tracksCount <= pageSize) return 1;
            return (int)Math.Round((double)(await _dbContext.Songs.CountAsync()) / pageSize);
        }

        public async Task<IEnumerable<PlaylistViewModel>> GetUserPlaylists(string userId = null)
        {
            // если id == null то получаем плейлисты всех юзеров
            IQueryable<Playlist> source = _dbContext.Playlists;
            if (userId != null)
            {
                source = source.Where(x => x.OwnerId == userId);
            }

            return await source.Select(x => new PlaylistViewModel
                { Id = x.Id, PathToCover = x.PathToCover, Name = x.Name}).ToListAsync();
        }

        public async Task<FilteredTracksViewModel> GetFilteredTracks(Songs.SearchModel model)
        {
            var tracks = _dbContext.Songs.Include(x => x.Author).Where(x =>
                (!model.FindBySongName || x.Name.Contains(model.SearchText)) &&
                (!model.FindByAuthorName || x.Author.Name.Contains(model.SearchText))).Select(x =>
                new TrackViewModel
                {
                    AuthorName = x.Author.Name,
                    Id = x.Id,
                    Name = x.Name,
                    PathToCover = x.PathToCover,
                    PathToTrack = x.PathToSong
                });
            var tracksCount = await tracks.CountAsync();
            return new FilteredTracksViewModel()
            {
                Tracks = await tracks.ToListAsync()
            };
        }
    }
}