using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using WebApplication1.Configurations;

namespace WebApplication1.Helpers
{
    public class TemporaryFileStorage
    {
        private readonly FileSettings _settings;
        private readonly IWebHostEnvironment _env;

        public TemporaryFileStorage(IOptionsMonitor<FileSettings> optionsMonitor, IWebHostEnvironment env)
        {
            _settings = optionsMonitor.CurrentValue;
            _env = env;
        }
        
        public async Task<string> SaveTemporaryFile(IFormFile video)
        {
            var fileName = string.Concat(
                TrickingLibraryConstants.Files.TempPrefix,
                DateTime.Now.Ticks,
                Path.GetExtension(video.FileName)
            );
            var savePath = GetSavePath(fileName);

            await using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                await video.CopyToAsync(fileStream);
            }

            return fileName;
        }

        public bool TemporaryFileExists(string fileName)
        {
            var path = GetSavePath(fileName);
            return File.Exists(path);
        }

        public void DeleteTemporaryFile(string fileName)
        {
            var path = GetSavePath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string GetSavePath(string fileName)
        {
            return Path.Combine(_env.WebRootPath, fileName);
        }
    }
}