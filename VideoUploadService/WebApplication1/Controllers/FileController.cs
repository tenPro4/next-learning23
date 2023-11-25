using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System.Threading.Channels;
using WebApplication1.BackgroundServices;
using WebApplication1.Configurations;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController: ControllerBase
    {
        private readonly TemporaryFileStorage _temporaryFileStorage;
        private readonly AppDbContext _context;
        private readonly IOptionsMonitor<FileSettings> _fileSettingsMonitor;
        private readonly IWebHostEnvironment _env;

        public FileController(TemporaryFileStorage temporaryFileStorage, AppDbContext context,
            IOptionsMonitor<FileSettings> fileSettingsMonitor,IWebHostEnvironment env)
        {
            _temporaryFileStorage = temporaryFileStorage;
            _context = context;
            _fileSettingsMonitor = fileSettingsMonitor;
            _env = env;
        }

        [HttpPost("video")]
        public async Task<IActionResult> UploadVideo(
            IFormFile video,
             [FromServices] Channel<EditVideoMessage> channel)
        {
            var _fileName = await _temporaryFileStorage.SaveTemporaryFile(video);

            if (!string.IsNullOrEmpty(_fileName) && !_temporaryFileStorage.TemporaryFileExists(_fileName))
            {
                return BadRequest();
            }

            var newUpload = new Upload
            {
                Done = false,
            };

            _context.Uploads.Add(newUpload);
            await _context.SaveChangesAsync();

            await channel.Writer.WriteAsync(new EditVideoMessage
            {
                Id = newUpload.Id,
                Input = _fileName,
            });

            return Ok(newUpload);
        }

        [HttpPost("file")]
        public async Task<string> UploadImage(IFormFile file)
        {
            var _fileName = await _temporaryFileStorage.SaveTemporaryFile(file);

            if (file.ContentType.StartsWith("image/"))
            {
                return $"{_fileSettingsMonitor.CurrentValue.ImageUrl}/{_fileName}";
            }

            return $"{_fileSettingsMonitor.CurrentValue.FileUrl}/{_fileName}";
        }

        [HttpGet("{type}/{file}")]
        public IActionResult GetVideo(string type, string file)
        {
            var mime = type.Equals(nameof(FileType.Image), StringComparison.InvariantCultureIgnoreCase)
            ? "image/jpg"
                : type.Equals(nameof(FileType.Video), StringComparison.InvariantCultureIgnoreCase)
                    ? "video/mp4"
                    : null;

            if (mime == null)
            {
                return BadRequest();
            }

            var savePath = _temporaryFileStorage.GetSavePath(file);
            if (string.IsNullOrEmpty(savePath))
            {
                return BadRequest();
            }

            return new FileStreamResult(new FileStream(savePath, FileMode.Open, FileAccess.Read), mime);
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", Path.GetFileName(filePath));
        }
    }
}
