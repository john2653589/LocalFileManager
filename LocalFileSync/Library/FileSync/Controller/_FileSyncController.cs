using Microsoft.AspNetCore.Mvc;
using Rugal.FileSync.Grpc;
using Rugal.FileSync.Model;

namespace Rugal.FileSync.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class _FileSyncController : ControllerBase
    {
        private readonly FileSyncClient SyncClient;
        public _FileSyncController(FileSyncClient _SyncClient)
        {
            SyncClient = _SyncClient;
        }

        [HttpPost]
        public async Task<FileSyncTradeResultModel> TrySyncToServer()
        {
            var SuccessCount = await SyncClient.TrySyncToServer();
            return SuccessCount;
        }

        [HttpPost]
        public async Task<FileSyncTradeResultModel> TrySyncFromServer()
        {
            var SuccessCount = await SyncClient.TrySyncFromServer();
            return SuccessCount;
        }

        [HttpPost]
        public async Task<FileSyncTradeResultModel> TrySyncTrade()
        {
            var SuccessResult = await SyncClient.TrySyncTrade();
            return SuccessResult;
        }

    }
}
