using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Rugal.LocalFileSync.Model;
using Rugal.LocalFileSync.Service;
using System.Reflection;

namespace Rugal.LocalFileSync.Grpc
{
    public class LocalFileSyncServer : SyncServer.SyncServerBase
    {
        private readonly SyncTradeService SyncTradeService;
        public LocalFileSyncServer(SyncTradeService _SyncTradeService)
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
            var ReceiveResult = await SyncTradeService.TryReceive(responseStream, requestStream);
            var SendResult = await SyncTradeService.TrySend(responseStream, requestStream);

            var Result = new SyncTradeResultModel()
            {
                SendCount = SendResult.SendCount,
                SendCheckCount = SendResult.SendCheckCount,
                ReceiveCount = ReceiveResult.ReceiveCount,
                ReceiveCheckCount = ReceiveResult.ReceiveCheckCount,
            };

            Console.WriteLine($"Data sync service finish {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine("\n=====Sync Result=====");
            Console.WriteLine($"SendCheckCount : {Result.SendCheckCount}");
            Console.WriteLine($"SendCount : {Result.SendCount}");
            Console.WriteLine($"ReceiveCheckCount : {Result.ReceiveCheckCount}");
            Console.WriteLine($"ReceiveCount : {Result.ReceiveCount}");
            Console.WriteLine("=====Sync Result=====\n");
        }
    }
}