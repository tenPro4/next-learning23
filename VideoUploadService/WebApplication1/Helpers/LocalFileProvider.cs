using Microsoft.Extensions.Options;
using WebApplication1.Configurations;

namespace WebApplication1.Helpers
{
    public class LocalFileProvider : IFileProvider
    {
        private readonly IWebHostEnvironment _env;
        private readonly FileSettings _settings;

        public LocalFileProvider(
            IOptionsMonitor<FileSettings> fileSettingsMonitor,
            IWebHostEnvironment env)
        {
            _settings = fileSettingsMonitor.CurrentValue;
            _env = env;
        }

        public async Task<string> SaveProfileImageAsync(Stream fileStream)
        {
            var fileName = TrickingLibraryConstants.Files.GenerateProfileFileName();
            await SaveFile(fileStream, fileName);
            return $"{_settings.ImageUrl}/{fileName}";
        }

        public async Task<string> SaveVideoAsync(Stream fileStream)
        {
            var fileName = TrickingLibraryConstants.Files.GenerateConvertedFileName();
            await SaveFile(fileStream, fileName);
            return $"{_settings.VideoUrl}/{fileName}";
        }

        public async Task<string> SaveThumbnailAsync(Stream fileStream)
        {
            var fileName = TrickingLibraryConstants.Files.GenerateThumbnailFileName();
            await SaveFile(fileStream, fileName);
            return $"{_settings.ImageUrl}/{fileName}";
        }

        private async Task SaveFile(Stream fileStream, string fileName)
        {
            var savePath = Path.Combine(_env.WebRootPath, fileName);
            await using (var stream = File.Create(savePath))
            {
                await fileStream.CopyToAsync(stream);
            }
        }
    }
}