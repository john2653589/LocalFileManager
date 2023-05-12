using Rugal.Net.LocalFileManager.Model;

namespace Rugal.LocalFileSync.Model
{
    public class SyncTradeModel
    {
        public SyncTradeType TradeType { get; set; }
        public LocalFileInfoModel FileInfo { get; set; }
        public long Length { get; set; }
        public byte[] Buffer { get; set; }
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
