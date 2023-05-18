
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

            while (!stoppingToken.IsCancellationRequested)
            {


                await Task.Delay(Setting.SyncPerMin.Value, stoppingToken);
            }
        }
    }
}
