using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EAD_MusicPlayer.Helpers
{
    public static class FileHelper
    {
        public static async Task SaveFile(IFormFile file, string filePath)
        {
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}