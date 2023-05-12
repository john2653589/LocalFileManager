
using Rugal.Net.LocalFileManager.Model;
using Rugal.Net.LocalFileManager.Service;

namespace Rugal.LocalFileSync.Service
{
    public class LocalFileSyncWorker : BackgroundService
    {
        private readonly LocalFileService LocalFileService;
        private LocalFileManagerSetting Setting => LocalFileService.Setting;
        public LocalFileSyncWorker(LocalFileService _LocalFileService)
        {
            LocalFileService = _LocalFileService;
        }
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
