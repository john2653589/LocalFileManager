using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Rugal.GrpcCommon.Extention;
using Rugal.LocalFileSync.Model;
using Rugal.Net.LocalFileManager.Model;
using Rugal.Net.LocalFileManager.Service;

namespace Rugal.LocalFileSync.Service
{
    public class SyncTradeService
    {
        private readonly LocalFileService LocalFileService;
        private LocalFileManagerSetting Setting => LocalFileService.Setting;
        public SyncTradeService(LocalFileService _LocalFileService)
        {
            LocalFileService = _LocalFileService;
        }
        public async Task TrySyncSend(IAsyncStreamWriter<Any> Sender, IAsyncStreamReader<Any> Receiver)
        {
            var FileList = LocalFileService.ForEachFile();
            var ErrorCount = 0;

            foreach (var File in FileList)
            {
                var IsSyncSuccess = false;
                while (!IsSyncSuccess)
                {
                    try
                    {
                        var FileWriter = new LocalFileWriter(LocalFileService, File);
                        await Sender.WriteAsync(new SyncTradeModel()
                        {
                            TradeType = SyncTradeType.Info,
                            FileInfo = File,
                            Length = File.Length,
                        }.ConvertToAny());
                        await Receiver.MoveNext();
                        var Model = Receiver.Current.ConvertBufferTo<SyncTradeModel>();

                        if (Model.TradeType == SyncTradeType.Info)
                        {
                            if (Model.Length == File.Length)
                                continue;

                            await Sender.WriteAsync(new SyncTradeModel()
                            {
                                TradeType = SyncTradeType.Create,
                                FileInfo = File,
                            }.ConvertToAny());
                        }
                        else if (Model.TradeType == SyncTradeType.HasTemp)
                        {
                            FileWriter.Seek(Model.Length);
                        }
                        else continue;

                        await FileWriter.ReadBytesAsync(async Buffer =>
                        {
                            await Sender.WriteAsync(new SyncTradeModel()
                            {
                                TradeType = SyncTradeType.Buffer,
                                Buffer = Buffer,
                                FileInfo = File,
                                Length = Buffer.Length,
                            }.ConvertToAny());

                            return true;
                        }, Setting.SyncToPerByte);
                        await Sender.WriteAsync(new SyncTradeModel()
                        {
                            TradeType = SyncTradeType.Pack,
                            FileInfo = File,
                        }.ConvertToAny());

                        IsSyncSuccess = true;
                        ErrorCount = 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        ErrorCount++;
                        if (ErrorCount >= 5)
                            break;
                    }
                }
                if (IsSyncSuccess)
                    break;
            }
            await TryWriteTrade(Sender, new SyncTradeModel()
            {
                TradeType = SyncTradeType.Complete,
            });
        }
        public async Task TryReceive(IAsyncStreamWriter<Any> Receiver, IAsyncStreamReader<Any> Sender)
        {
            using var Writer = new LocalFileWriter(LocalFileService);
            var IsTrying = true;
            var ErrorCount = 0;
            while (IsTrying)
            {
                try
                {
                    var IsNext = await Sender.MoveNext();
                    if (!IsNext)
                        break;

                    var Model = Sender.Current.ConvertBufferTo<SyncTradeModel>();
                    switch (Model.TradeType)
                    {
                        case SyncTradeType.Info:
                            await TradeInfo(Writer, Model, Receiver);
                            break;
                        case SyncTradeType.Create:
                            Writer.WithTemp();
                            break;
                        case SyncTradeType.Buffer:
                            Writer.WriteBytes(Model.Buffer, Model.Length);
                            break;
                        case SyncTradeType.Pack:
                            Writer.WithRemoveTemp()
                                .ClearStream();
                            break;
                        case SyncTradeType.Complete:
                            IsTrying = false;
                            break;
                    }
                    ErrorCount = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    ErrorCount++;
                    if (ErrorCount >= 5)
                        IsTrying = false;
                }
            }
        }
        private static async Task TradeInfo(LocalFileWriter Writer, SyncTradeModel Model, IAsyncStreamWriter<Any> StreamWriter)
        {
            Writer.WithFile(Model.FileInfo);

            var IsTemp = Writer.IsHasTemp();
            if (IsTemp)
                Writer.WithTemp();

            await StreamWriter.WriteAsync(new SyncTradeModel()
            {
                TradeType = IsTemp ? SyncTradeType.HasTemp : SyncTradeType.Info,
                Length = Writer.Length,
            }.ConvertToAny());
        }
        public async Task<bool> TryWriteTrade(IAsyncStreamWriter<Any> StreamWriter, SyncTradeModel TradeModel)
        {
            try
            {
                await StreamWriter.WriteAsync(TradeModel.ConvertToAny());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}