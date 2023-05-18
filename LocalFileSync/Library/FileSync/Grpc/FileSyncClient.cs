using Grpc.Net.Client;
using Rugal.FileSync.Model;
using Rugal.FileSync.Service;
using Rugal.Net.LocalFileManager.Model;
using Rugal.Net.LocalFileManager.Service;

namespace Rugal.FileSync.Grpc
{
    public class FileSyncClient
    {
        private readonly LocalFileService LocalFileService;
        private readonly FileSyncTradeService SyncTradeService;
        private LocalFileManagerSetting Setting => LocalFileService.Setting;
        private FileSync.FileSyncClient Client { get; set; }
        public FileSyncClient(LocalFileService _LocalFileService, FileSyncTradeService _SyncTradeService)
        {
            LocalFileService = _LocalFileService;
            SyncTradeService = _SyncTradeService;
            CreateClient();
        }
        public void CreateClient()
        {
            var GetChannel = GrpcChannel.ForAddress(Setting.RemoteDomain);
            Client = new FileSync.FileSyncClient(GetChannel);
        }

        public async Task<FileSyncTradeResultModel> TrySyncToServer()
        {
            var Server = Client.SyncToServer();
            var Receiver = Server.ResponseStream;
            var Sender = Server.RequestStream;

            var SuccessCount = await SyncTradeService.TrySend(Sender, Receiver);
            await Sender.CompleteAsync();
            return SuccessCount;
        }
        public async Task<FileSyncTradeResultModel> TrySyncFromServer()
        {
            var Server = Client.SyncFromServer();
            var Response = Server.ResponseStream;
            var Rquest = Server.RequestStream;

            var SuccessCount = await SyncTradeService.TryReceive(Rquest, Response);
            await Rquest.CompleteAsync();
            return SuccessCount;
        }
        public async Task<FileSyncTradeResultModel> TrySyncTrade()
        {
            var Server = Client.SyncTrade();
            var Receiver = Server.ResponseStream;
            var Sender = Server.RequestStream;

            var SendResult = await SyncTradeService.TrySend(Sender, Receiver);
            var ReceiveResult = await SyncTradeService.TryReceive(Sender, Receiver);

            await Sender.CompleteAsync();
            return new FileSyncTradeResultModel()
            {
                SendCount = SendResult.SendCount,
                SendCheckCount = SendResult.SendCheckCount,
                ReceiveCount = ReceiveResult.ReceiveCount,
                ReceiveCheckCount = ReceiveResult.ReceiveCheckCount,
            };
        }
    }
}