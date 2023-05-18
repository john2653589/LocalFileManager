
using Rugal.LocalFileSync.Grpc;
using Rugal.Net.LocalFileManager.Model;
using Rugal.Net.LocalFileManager.Service;

namespace Rugal.LocalFileSync.Service
{
    public class LocalFileSyncWorker : BackgroundService
    {
        private LocalFileManagerSetting Setting => LocalFileService.Setting;
        private readonly LocalFileService LocalFileService;
        private readonly LocalFileSyncClient SyncClient;
        public LocalFileSyncWorker(LocalFileService _LocalFileService, LocalFileSyncClient _SyncClient)
        {
            LocalFileService = _LocalFileService;
            SyncClient = _SyncClient;
        }
        public LocalFileSyncWorker() { }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (Setting.SyncPerMin == null)
                return;

            if (Setting.SyncWay == SyncWayType.None)
                return;

            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"---Data sync service run {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                var Result = Setting.SyncWay switch
                {
                    SyncWayType.ToServer => await SyncClient.TrySyncToServer(),
                    SyncWayType.FromServer => await SyncClient.TrySyncFromServer(),
                    SyncWayType.Trade => await SyncClient.TrySyncTrade(),
                    _ => null
                };
                if (Result is null)
                    return;

                Console.WriteLine($"---Data sync service finish {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine("\n=====Sync Result=====");
                Console.WriteLine($"SendCheckCount : {Result.SendCheckCount}");
                Console.WriteLine($"SendCount : {Result.SendCount}");
                Console.WriteLine($"ReceiveCheckCount : {Result.ReceiveCheckCount}");
                Console.WriteLine($"ReceiveCount : {Result.ReceiveCount}");
                Console.WriteLine("=====Sync Result=====\n");

                await Task.Delay(Setting.SyncPerMin.Value, stoppingToken);
            }
        }
    }
}
