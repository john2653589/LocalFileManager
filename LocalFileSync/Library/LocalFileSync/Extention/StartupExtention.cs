using Rugal.LocalFileSync.Service;
using System.Runtime.InteropServices;

namespace Rugal.LocalFileSync.Extention
{
    public static class StartupExtention
    {
        public static void AddLocalFileSyncService(this IHostBuilder Host)
        {
            var IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if (IsWindows)
                Host.UseWindowsService(options =>
                {
                    options.ServiceName = "LocalFileSync Service";
                });
            else if (IsLinux)
                Host.UseSystemd();

            Host.ConfigureServices(Services =>
            {
                Services.AddHostedService<LocalFileSyncWorker>();
            });
        }
    }
}