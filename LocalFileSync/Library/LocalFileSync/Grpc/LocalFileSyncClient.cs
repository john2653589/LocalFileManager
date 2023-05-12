using Grpc.Net.Client;
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

        public async Task TrySyncToServer()
        {
            var Server = Client.SyncToServer();
            var Receiver = Server.ResponseStream;
            var Sender = Server.RequestStream;

            await SyncTradeService.TrySyncSend(Sender, Receiver);
            await Sender.CompleteAsync();

        }
        public async Task TrySyncFromServer()
        {
            var Server = Client.SyncFromServer();
            var Response = Server.ResponseStream;
            var Rquest = Server.RequestStream;

            await SyncTradeService.TryReceive(Rquest, Response);
            await Rquest.CompleteAsync();
        }
        public async Task TrySyncTrade()
        {
            var Server = Client.SyncTrade();
            var Receiver = Server.ResponseStream;
            var Sender = Server.RequestStream;

            await SyncTradeService.TrySyncSend(Sender, Receiver);
            await SyncTradeService.TryReceive(Sender, Receiver);

            await Sender.CompleteAsync();
        }
    }
}