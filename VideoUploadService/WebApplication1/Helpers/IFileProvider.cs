using System.IO;
using System.Threading.Tasks;

namespace WebApplication1.Helpers
{
    public interface IFileProvider
    {
        public Task<string> SaveProfileImageAsync(Stream fileStream);
        public Task<string> SaveVideoAsync(Stream fileStream);
        public Task<string> SaveThumbnailAsync(Stream fileStream);
    }
}