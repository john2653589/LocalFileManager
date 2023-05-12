namespace Rugal.NetCommon.Extention.Param
{
    public static class ParamExtention
    {
        public static bool IsHasValue(this Guid? CheckGuid)
        {
            if (CheckGuid is null)
                return false;

            if (CheckGuid == Guid.Empty)
                return false;

            return true;
        }

        public static bool IsHasValue(this DateTime? CheckDatetime)
        {
            if (CheckDatetime is null)
                return false;

            if (CheckDatetime == DateTime.MinValue)
                return false;

            return true;
        }

        public static DateTime ToClearDate(this DateTime ConvertDatetime)
        {
            var ClearDate = new DateTime(ConvertDatetime.Year, ConvertDatetime.Month, ConvertDatetime.Day);
            return ClearDate;
        }

        public static bool IsHasValue(this string CheckString)
        {
            if (string.IsNullOrWhiteSpace(CheckString))
                return false;

            return true;
        }

        public static bool IsHasValue(this bool? CheckBool)
        {
            if (CheckBool is null)
                return false;
            return (bool)CheckBool;
        }

        public static bool IsHasValue(this int? CheckInt)
        {
            if (CheckInt is null)
                return false;
            if (CheckInt <= 0)
                return false;
            return true;
        }

        public static bool IsHasValue(this int CheckInt)
        {
            if (CheckInt <= 0)
                return false;
            return true;
        }
        public static bool IsHasValue_CanZero(this int? CheckInt)
        {
            if (CheckInt is null)
                return false;
            return CheckInt >= 0;
        }
        public static bool IsHasValue_CanZero(this int CheckInt)
        {
            return CheckInt >= 0;
        }

    }
}
