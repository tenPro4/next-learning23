using WebApplication1;
using WebApplication1.Configurations;
using WebApplication1.Helpers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegisterService
    {
        public static IServiceCollection AddFileServices(this IServiceCollection services, IConfiguration config)
        {
            var settingsSection = config.GetSection(nameof(FileSettings));
            var settings = settingsSection.Get<FileSettings>();
            services.Configure<FileSettings>(settingsSection);

            services.AddSingleton<TemporaryFileStorage>();

            if (settings.Provider.Equals(TrickingLibraryConstants.Files.Providers.Local))
            {
                services.AddSingleton<IFileProvider, LocalFileProvider>();
            }
            else
            {
                throw new Exception($"Invalid File Manager Provider: {settings.Provider}");
            }

            return services;
        }
    }
}
