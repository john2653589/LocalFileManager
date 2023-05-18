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
        public async Task<SyncTradeResultModel> TrySend(IAsyncStreamWriter<Any> Sender, IAsyncStreamReader<Any> Receiver)
        {
            var FileList = LocalFileService.ForEachFiles();
            var ErrorCount = 0;
            var SuccessCount = 0;
            var FileCount = 0;
            foreach (var File in FileList)
            {
                FileCount++;
                var IsSyncSuccess = false;
                while (!IsSyncSuccess)
                {
                    try
                    {
                        using var FileWriter = new LocalFileWriter(LocalFileService, File);
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
                            {
                                IsSyncSuccess = true;
                                continue;
                            }

                            await Sender.WriteAsync(new SyncTradeModel()
                            {
                                TradeType = SyncTradeType.Create,
                                FileInfo = File,
                            }.ConvertToAny());
                        }
                        else if (Model.TradeType == SyncTradeType.HasTemp)
                        {
                            if (Model.Length == File.Length)
                            {
                                await Sender.WriteAsync(new SyncTradeModel()
                                {
                                    TradeType = SyncTradeType.Pack,
                                    FileInfo = File,
                                }.ConvertToAny());
                                IsSyncSuccess = true;
                                continue;
                            }
                            FileWriter.OpenRead()
                                .Seek(Model.Length);
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
                        Console.WriteLine($"Send file: 「{File.FileName}」 from 「{File.FullPath}」");

                        SuccessCount++;
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
            }
            try
            {
                await TryWriteTrade(Sender, new SyncTradeModel()
                {
                    TradeType = SyncTradeType.Complete,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var Result = new SyncTradeResultModel()
            {
                SendCount = SuccessCount,
                SendCheckCount = FileCount,
            };
            return Result;
        }
        public async Task<SyncTradeResultModel> TryReceive(IAsyncStreamWriter<Any> Receiver, IAsyncStreamReader<Any> Sender)
        {
            var Writer = new LocalFileWriter(LocalFileService);
            var IsTrying = true;
            var ErrorCount = 0;
            var SuccessCount = 0;
            var FileCount = 0;
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
                            FileCount++;
                            await TradeInfo(Writer, Model, Receiver);
                            break;
                        case SyncTradeType.Create:
                            Writer.WithTemp();
                            break;
                        case SyncTradeType.Buffer:
                            Writer.WriteBytes(Model.Buffer, Model.Length);
                            break;
                        case SyncTradeType.Pack:
                            Writer.WithRemoveTemp();
                            Console.WriteLine($"Receive file: 「{Model.FileInfo.FileName}」 from 「{Model.FileInfo.FullPath}」");
                            SuccessCount++;
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

            Writer.Dispose();

            var Result = new SyncTradeResultModel()
            {
                ReceiveCount = SuccessCount,
                ReceiveCheckCount = FileCount,
            };
            return Result;
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