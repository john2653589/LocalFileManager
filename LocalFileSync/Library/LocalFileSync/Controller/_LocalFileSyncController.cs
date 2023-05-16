using Microsoft.AspNetCore.Mvc;
using Rugal.LocalFileSync.Grpc;
using Rugal.LocalFileSync.Model;
using Rugal.Net.LocalFileManager.Service;

namespace Rugal.LocalFileSync.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class _LocalFileSyncController : ControllerBase
    {
        private readonly LocalFileSyncClient SyncClient;
        public _LocalFileSyncController(LocalFileSyncClient _SyncClient)
        {
            SyncClient = _SyncClient;
        }

        [HttpPost]
        public async Task<SyncTradeResultModel> TrySyncToServer()
        {
            var SuccessCount = await SyncClient.TrySyncToServer();
            return SuccessCount;
        }

        [HttpPost]
        public async Task<SyncTradeResultModel> TrySyncFromServer()
        {
            var SuccessCount = await SyncClient.TrySyncFromServer();
            return SuccessCount;
        }

        [HttpPost]
        public async Task<SyncTradeResultModel> TrySyncTrade()
        {
            var SuccessResult = await SyncClient.TrySyncTrade();
            return SuccessResult;
        }

    }
}
