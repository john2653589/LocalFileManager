using Grpc.Net.Client;
using Rugal.LocalFileSync.Model;
using Rugal.LocalFileSync.Service;
using Rugal.Net.LocalFileManager.Model;
using Rugal.Net.LocalFileManager.Service;

namespace Rugal.LocalFileSync.Grpc
{
    public class LocalFileSyncClient
    {
        private readonly LocalFileService LocalFileService;
        private readonly SyncTradeService SyncTradeService;
        private LocalFileManagerSetting Setting => LocalFileService.Setting;
        private SyncServer.SyncServerClient Client { get; set; }
        public LocalFileSyncClient(LocalFileService _LocalFileService, SyncTradeService _SyncTradeService)
        {
            LocalFileService = _LocalFileService;
            SyncTradeService = _SyncTradeService;
            CreateClient();
        }
        public void CreateClient()
        {
            var GetChannel = GrpcChannel.ForAddress(Setting.RemoteDomain);
            Client = new SyncServer.SyncServerClient(GetChannel);
        }

        public async Task<SyncTradeResultModel> TrySyncToServer()
        {
            var Server = Client.SyncToServer();
            var Receiver = Server.ResponseStream;
            var Sender = Server.RequestStream;

            var SuccessCount = await SyncTradeService.TrySyncSend(Sender, Receiver);
            await Sender.CompleteAsync();
            return SuccessCount;
        }
        public async Task<SyncTradeResultModel> TrySyncFromServer()
        {
            var Server = Client.SyncFromServer();
            var Response = Server.ResponseStream;
            var Rquest = Server.RequestStream;

            var SuccessCount = await SyncTradeService.TryReceive(Rquest, Response);
            await Rquest.CompleteAsync();
            return SuccessCount;
        }
        public async Task<SyncTradeResultModel> TrySyncTrade()
        {
            var Server = Client.SyncTrade();
            var Receiver = Server.ResponseStream;
            var Sender = Server.RequestStream;

            var SendResult = await SyncTradeService.TrySyncSend(Sender, Receiver);
            var ReceiveResult = await SyncTradeService.TryReceive(Sender, Receiver);

            await Sender.CompleteAsync();
            return new SyncTradeResultModel()
            {
                SendCount = SendResult.SendCount,
                SendCheckCount = SendResult.SendCheckCount,
                ReceiveCount = ReceiveResult.ReceiveCount,
                ReceiveCheckCount = ReceiveResult.ReceiveCheckCount,
            };
        }
    }
}