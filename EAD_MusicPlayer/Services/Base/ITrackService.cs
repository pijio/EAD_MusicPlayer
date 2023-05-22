using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EAD_MusicPlayer.Data.DomainModels;
using EAD_MusicPlayer.Models;

namespace EAD_MusicPlayer.Services.Base
{
    /// <summary>
    /// Сервис для работы с треками
    /// </summary>
    public interface ITrackService
    {
        /// <summary>
        /// Получение всех треков
        /// </summary>
        Task<List<TrackViewModel>> GetAllTracks();

        /// <summary>
        /// Получение треков плейлиста
        /// </summary>
        Task<List<TrackViewModel>> GetPlaylistTrack(string playListId);

        /// <summary>
        /// Получение треков с помощью пагинации
        /// </summary>
        /// <param name="pageNo">Номер страницы</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns></returns>
        Task<List<TrackViewModel>> GetTracksPage(int pageNo, int pageSize);

        /// <summary>
        /// Количество страниц с треками
        /// </summary>
        /// <returns></returns>
        Task<int> GetPagesCount(int pageSize);

        /// <summary>
        /// Получение плейлистов
        /// </summary>
        /// <param name="id">id плейлиста</param>
        /// <returns></returns>
        Task<IEnumerable<PlaylistViewModel>> GetUserPlaylists(string id = null);
    }
}