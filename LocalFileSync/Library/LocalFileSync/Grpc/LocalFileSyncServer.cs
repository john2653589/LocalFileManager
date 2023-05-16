using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Rugal.LocalFileSync.Service;

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
            await SyncTradeService.TryReceive(responseStream, requestStream);
            await SyncTradeService.TrySend(responseStream, requestStream);
        }
    }
}