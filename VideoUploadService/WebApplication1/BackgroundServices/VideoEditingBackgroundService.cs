using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading.Channels;
using WebApplication1.Configurations;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.BackgroundServices
{
    public class VideoEditingBackgroundService : BackgroundService
    {
        private readonly ILogger<VideoEditingBackgroundService> _logger;
        private readonly IOptionsMonitor<FileSettings> _fileSettingsMonitor;
        private readonly IServiceProvider _serviceProvider;
        private readonly TemporaryFileStorage _temporaryFileStorage;
        private readonly IFileProvider _fileProvider;
        private readonly ChannelReader<EditVideoMessage> _channelReader;

        public VideoEditingBackgroundService(
            Channel<EditVideoMessage> channel,
            ILogger<VideoEditingBackgroundService> logger,
            IOptionsMonitor<FileSettings> fileSettingsMonitor,
            IServiceProvider serviceProvider,
            TemporaryFileStorage temporaryFileStorage,
            IFileProvider fileProvider)
        {
            _logger = logger;
            _fileSettingsMonitor = fileSettingsMonitor;
            _serviceProvider = serviceProvider;
            _temporaryFileStorage = temporaryFileStorage;
            _fileProvider = fileProvider;
            _channelReader = channel.Reader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _channelReader.WaitToReadAsync(stoppingToken))
            {
                var message = await _channelReader.ReadAsync(stoppingToken);
                var inputPath = _temporaryFileStorage.GetSavePath(message.Input);
                var outputConvertedName = TrickingLibraryConstants.Files.GenerateConvertedFileName();
                var outputThumbnailName = TrickingLibraryConstants.Files.GenerateThumbnailFileName();
                var outputConvertedPath = _temporaryFileStorage.GetSavePath(outputConvertedName);
                var outputThumbnailPath = _temporaryFileStorage.GetSavePath(outputThumbnailName);

                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = _fileSettingsMonitor.CurrentValue.FFMPEGPath,
                        Arguments = $"-y -i {inputPath} -an -vf scale=540x380 {outputConvertedPath} -ss 00:00:00 -vframes 1 -vf scale=540x380 {outputThumbnailPath}",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    };

                    using (var process = new Process { StartInfo = startInfo })
                    {
                        process.Start();
                        process.WaitForExit();
                    }

                    if (!_temporaryFileStorage.TemporaryFileExists(outputConvertedName))
                    {
                        throw new Exception("FFMPEG failed to generate converted video");
                    }

                    if (!_temporaryFileStorage.TemporaryFileExists(outputThumbnailName))
                    {
                        throw new Exception("FFMPEG failed to generate thumbnail");
                    }

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var upload = ctx.Uploads.FirstOrDefault(x => x.Id.Equals(message.Id));

                        //using (var videoStream = File.Open(outputConvertedPath, FileMode.Open, FileAccess.Read))
                        //using (var thumbnailStream = File.Open(outputThumbnailPath, FileMode.Open, FileAccess.Read))
                        //{
                        //    var videoLink = _fileProvider.SaveVideoAsync(videoStream);
                        //    var thumbLink = _fileProvider.SaveThumbnailAsync(thumbnailStream);
                        //    upload.VideoLink = await videoLink;
                        //    upload.ThumbLink = await thumbLink;
                        //}

                        if(upload != null)
                        {
                            upload.VideoLink = $"{_fileSettingsMonitor.CurrentValue.VideoUrl}/{outputConvertedName}";
                            upload.ThumbLink = $"{_fileSettingsMonitor.CurrentValue.ImageUrl}/{outputThumbnailName}";
                            upload.Done = true;

                            await ctx.SaveChangesAsync(stoppingToken);
                        }
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Video Processing Failed for {0}", message.Input);
                }
                finally
                {
                    //_temporaryFileStorage.DeleteTemporaryFile(outputConvertedName);
                    //_temporaryFileStorage.DeleteTemporaryFile(outputThumbnailName);
                    _temporaryFileStorage.DeleteTemporaryFile(message.Input);
                }

                Console.WriteLine(message.Input);
            }
        }
    }
}
