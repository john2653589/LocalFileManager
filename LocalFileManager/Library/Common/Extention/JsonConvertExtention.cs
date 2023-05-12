using Rugal.NetCommon.JsonConverts;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rugal.NetCommon.Extention.JsonConvert
{
    public static class JsonConvertExtention
    {
        public static string GetUTF8String(this Utf8JsonReader Reader)
        {
            var Ret = Encoding.UTF8.GetString(Reader.ValueSpan);
            return Ret;
        }
        public static JsonConverter[] GetAllConvert()
        {
            var Ret = new JsonConverter[]
            {
                new TimeSpanJsonConvert(),
                new DateTimeJsonConvert(),
                new DateTimeNullJsonConvert(),
                new BooleanJsonConvert(),
                new BooleanNullJsonConvert(),
                new GuidNullJsonConvert(),
                new GuidJsonConvert(),
                new IntJsonConvert(),
                new IntNullJsonConvert(),
            };
            return Ret;
        }
        public static void AddAllConvert(this IList<JsonConverter> ConvertList)
        {
            var AddConvert = GetAllConvert();
            foreach (var Item in AddConvert)
                ConvertList.Add(Item);
        }
    }
}
