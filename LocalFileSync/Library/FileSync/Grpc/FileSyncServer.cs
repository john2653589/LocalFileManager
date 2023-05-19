using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Rugal.FileSync.Model;
using Rugal.FileSync.Service;

namespace Rugal.FileSync.Grpc
{
    public class FileSyncServer : FileSync.FileSyncBase
    {
        private readonly FileSyncTradeService SyncTradeService;
        public FileSyncServer(FileSyncTradeService _SyncTradeService)
        {
            SyncTradeService = _SyncTradeService;
        }

        public override async Task SyncToServer(IAsyncStreamReader<Any> requestStream, IServerStreamWriter<Any> responseStream, ServerCallContext context)
        {
            await SyncTradeService.TryReceive(responseStream, requestStream);
        }

        public override async Task SyncFromServer(IAsyncStreamReader<Any> requestStream, IServerStreamWriter<Any> responseStream, ServerCallContext context)
        {
            await SyncTradeService.TrySend(responseStream, requestStream);
        }

        public override async Task SyncTrade(IAsyncStreamReader<Any> requestStream, IServerStreamWriter<Any> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"---File sync service run {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            var ReceiveResult = await SyncTradeService.TryReceive(responseStream, requestStream);
            var SendResult = await SyncTradeService.TrySend(responseStream, requestStream);

            var Result = new FileSyncTradeResultModel()
            {
                SendCount = SendResult.SendCount,
                SendCheckCount = SendResult.SendCheckCount,
                ReceiveCount = ReceiveResult.ReceiveCount,
                ReceiveCheckCount = ReceiveResult.ReceiveCheckCount,
            };

            Console.WriteLine($"---File sync service finish {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            Console.WriteLine("\n=====Sync Result=====");
            Console.WriteLine($"SendCheckCount : {Result.SendCheckCount}");
            Console.WriteLine($"SendCount : {Result.SendCount}");
            Console.WriteLine($"ReceiveCheckCount : {Result.ReceiveCheckCount}");
            Console.WriteLine($"ReceiveCount : {Result.ReceiveCount}");
            Console.WriteLine("=====Sync Result=====\n");
        }
    }
}