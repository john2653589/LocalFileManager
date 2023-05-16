using Rugal.LocalFileSync.Grpc;
using Rugal.LocalFileSync.Service;
using Rugal.Net.LocalFileManager.Service;
using System.Runtime.InteropServices;

namespace Rugal.LocalFileSync.Extention
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
                    options.ServiceName = "LocalFileSync Service";
                });
            else if (IsLinux)
                Host.UseSystemd();

            Host.ConfigureServices(Services =>
            {
                Services.AddHostedService<LocalFileSyncWorker>();
            });
        }

        public static IServiceCollection AddLocalFileSyncTrade(this IServiceCollection Services)
        {
            Services.AddScoped<SyncTradeService>();
            return Services;
        }
        public static IServiceCollection LocalFileSyncClient(this IServiceCollection Services)
        {
            AddLocalFileSyncTrade(Services);
            Services.AddGrpc();
            Services.AddScoped<LocalFileSyncClient>();
            return Services;
        }


        public static GrpcServiceEndpointConventionBuilder MapLocalFileSyncServer(this WebApplication App)
        {
            var Builder = App.MapGrpcService<LocalFileSyncServer>();
            return Builder;
        }
    }
}