using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.Text.Json;

namespace Rugal.GrpcCommon.Extention
{
    public static class GrpcExtention
    {
        public static ByteString ConvertSendBuffer(this object ConvertObject)
        {
            var GetBuffer = JsonSerializer.Serialize(ConvertObject);
            var SendBuffer = ByteString.CopyFromUtf8(GetBuffer);
            return SendBuffer;
        }
        public static Any ConvertToAny(this object ConvertObject)
        {
            var ReturnAny = new Any()
            {
                Value = ConvertObject.ConvertSendBuffer(),
            };
            return ReturnAny;
        }
        public static TResult ConvertBufferTo<TResult>(this ByteString Buffer)
        {
            var GetJson = Buffer.ToStringUtf8();
            var GetResult = JsonSerializer.Deserialize<TResult>(GetJson);
            return GetResult;
        }
        public static object ConvertBufferTo(this ByteString Buffer, System.Type ConvertType)
        {
            var GetJson = Buffer.ToStringUtf8();
            var GetResult = JsonSerializer.Deserialize(GetJson, ConvertType);
            return GetResult;
        }
        public static TResult ConvertBufferTo<TResult>(this Any Buffer)
        {
            var GetResult = Buffer.Value.ConvertBufferTo<TResult>();
            return GetResult;
        }
        public static object ConvertBufferTo(this Any Buffer, System.Type ConvertType)
        {
            var GetResult = Buffer.Value.ConvertBufferTo(ConvertType);
            return GetResult;
        }
    }
}