using Rugal.FileSync.Grpc;
using Rugal.FileSync.Service;
using System.Runtime.InteropServices;

namespace Rugal.FileSync.Extention
{
    public static class StartupExtention
    {
        public static void AddLocalFileSyncService(this IHostBuilder Host, IServiceCollection Services)
        {
            var IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            AddLocalFileSyncTrade(Services);

            if (IsWindows)
                Host.UseWindowsService(options =>
                {
                    options.ServiceName = "FileSync Service";
                });
            else if (IsLinux)
                Host.UseSystemd();

            Host.ConfigureServices(Services =>
            {
                Services.AddHostedService<FileSyncWorker>();
            });
        }

        public static IServiceCollection AddLocalFileSyncTrade(this IServiceCollection Services)
        {
            Services.AddSingleton<FileSyncTradeService>();
            return Services;
        }
        public static IServiceCollection LocalFileSyncClient(this IServiceCollection Services)
        {
            AddLocalFileSyncTrade(Services);
            Services.AddGrpc();
            Services.AddSingleton<FileSyncClient>();
            return Services;
        }


        public static GrpcServiceEndpointConventionBuilder MapLocalFileSyncServer(this WebApplication App)
        {
            var Builder = App.MapGrpcService<FileSyncServer>();
            return Builder;
        }
    }
}