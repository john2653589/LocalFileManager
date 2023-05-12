using System.Globalization;

namespace Rugal.NetCommon.Extention.DateTimeParser
{
	public sealed class DateTimeParse
	{
		public static CultureInfo TwCulture => GetTwCulture();
		public static DateTime ParseTwToAd(string TwDateTime)
		{
			var GetTwDateTime = DateTime.Parse(TwDateTime, TwCulture);
			return GetTwDateTime;
		}
		public static bool TryParseTwToAd(string TwDateTime, out DateTime GetTwDateTime)
		{
			var IsParse = DateTime.TryParse(TwDateTime, TwCulture, DateTimeStyles.None, out GetTwDateTime);
			return IsParse;
		}
		private static CultureInfo GetTwCulture()
		{
			var GetTwCulture = new CultureInfo("zh-TW");
			GetTwCulture.DateTimeFormat.Calendar = new TaiwanCalendar();
			return GetTwCulture;
		}
	}

	public static class DateTimeExtention
	{
		public static string ToString_Tw(this DateTime AdDateTime, string Format)
		{
			var TwCulture = DateTimeParse.TwCulture;
			var FormatDatetime = AdDateTime.ToString(Format, TwCulture);
			return FormatDatetime;
		}
	}
}
