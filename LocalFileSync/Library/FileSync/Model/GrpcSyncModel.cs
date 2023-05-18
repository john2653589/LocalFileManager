using Rugal.Net.LocalFileManager.Model;

namespace Rugal.FileSync.Model
{
    public class SyncTradeModel
    {
        public SyncTradeType TradeType { get; set; }
        public LocalFileInfoModel FileInfo { get; set; }
        public long Length { get; set; }
        public byte[] Buffer { get; set; }
    }
    public class SyncTradeResultModel
    {
        public int SendCount { get; set; }
        public int SendCheckCount { get; set; }
        public int ReceiveCount { get; set; }
        public int ReceiveCheckCount { get; set; }
    }
    public enum SyncTradeType
    {
        Info = 0,
        HasTemp = 1,
        Create = 2,
        Buffer = 3,
        Pack = 4,
        Complete = 5,
    }
}
