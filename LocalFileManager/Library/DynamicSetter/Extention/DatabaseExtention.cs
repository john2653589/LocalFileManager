using Microsoft.EntityFrameworkCore;

namespace Rugal.Net.DynamicSetter.Extention
{
    public static class DatabaseExtention
    {
        public static IQueryable<object> Table(this DbContext DbContext, string TableName)
        {
            var TableProperty = DbContext
                .GetType()
                .GetProperties()
                .Where(Item =>
                {
                    var GetInnerType = Item
                        .GetValue(DbContext)
                        .GetType()
                        .GetGenericArguments()
                        .FirstOrDefault();

                    if (GetInnerType is null)
                        return false;

                    var IsMatch = GetInnerType.Name.ToLower() == TableName.ToLower();
                    return IsMatch;
                })
                .FirstOrDefault();

            if (TableProperty is null)
                return default;

            var GetTable = TableProperty.GetValue(DbContext) as IQueryable<object>;
            return GetTable;
        }

        public static object CreateInnerData(this object Table)
        {
            var TableType = Table.GetType();
            var GetDataType = TableType.GenericTypeArguments.FirstOrDefault();
            if (GetDataType is null)
                return null;

            var NewData = Activator.CreateInstance(GetDataType);
            return NewData;
        }

    }
}
