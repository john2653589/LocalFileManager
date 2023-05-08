using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rugal.Net.DynamicSetter.Model;
using System.Linq.Expressions;

namespace Rugal.Net.DynamicSetter.Extention
{
    public static class PropertyExtention
    {
        public static object GetPropertyValue<TModel>(this TModel Obj, string PropertyName, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            if (PropertyName is null)
                return default;

            var AllProperty = Obj.GetType().GetProperties();
            var Property = AllProperty
                .Where(Item =>
                {
                    var GetName = Item.Name;
                    var SetName = PropertyName;
                    if (CaseCompare == CaseCompareType.None)
                    {
                        GetName = GetName.ToLower();
                        SetName = SetName.ToLower();
                    }
                    return GetName == SetName;
                })
                .FirstOrDefault();

            var Value = Property?.GetValue(Obj);
            return Value;
        }
        public static TValue GetPropertyValue<TValue>(this object Obj, string PropertyName, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            if (PropertyName is null)
                return default;

            var GetValue = Obj.GetPropertyValue(PropertyName, CaseCompare);
            if (GetValue == null)
                return default;

            try
            {
                var Value = (TValue)GetValue;
                return Value;
            }
            catch { }

            try
            {
                var Value = (TValue)Convert.ChangeType(GetValue, typeof(TValue));
                return Value;
            }
            catch { }

            return default;
        }

        public static bool TryGetPropertyValue(this object Obj, string PropertyName, out object OutValue, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            OutValue = null;

            if (string.IsNullOrWhiteSpace(PropertyName))
                return false;

            var AllProperty = Obj.GetType().GetProperties();
            var Property = AllProperty
                .Where(Item =>
                {
                    var GetName = Item.Name;
                    var SetName = PropertyName;
                    if (CaseCompare == CaseCompareType.None)
                    {
                        GetName = GetName.ToLower();
                        SetName = SetName.ToLower();
                    }
                    return GetName == SetName;
                })
                .FirstOrDefault();

            if (Property is null)
                return false;

            OutValue = Property.GetValue(Obj);
            return true;
        }
        public static bool TryGetPropertyValue<TValue>(this object Obj, string PropertyName, out TValue OutValue, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            OutValue = default;
            if (!Obj.TryGetPropertyValue(PropertyName, out var GetValue, CaseCompare))
                return false;

            OutValue = (TValue)Convert.ChangeType(GetValue, typeof(TValue));
            return true;
        }

        public static TModel SetPropertyValue<TModel>(this TModel Obj, string PropertyName, object SetValue, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            if (PropertyName is null)
                return Obj;

            var AllProperty = Obj.GetType().GetProperties();
            var Property = AllProperty
                .Where(Item =>
                {
                    var GetName = Item.Name;
                    var SetName = PropertyName;
                    if (CaseCompare == CaseCompareType.None)
                    {
                        GetName = GetName.ToLower();
                        SetName = SetName.ToLower();
                    }
                    return GetName == SetName;
                })
                .FirstOrDefault();

            if (Property is null)
                return Obj;

            Property.SetValue(Obj, SetValue);

            return Obj;
        }

        public static TModel SetPropertyValue<TModel, TValue>(this TModel Obj, Expression<Func<TModel, TValue>> Exp, object SetValue, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            var AllPropertyName = Exp.GetMembers();
            var SetValueType = SetValue.GetType();
            foreach (var Item in AllPropertyName)
            {
                var PropertyName = Item.Name;
                if (!SetValueType.IsClass || SetValue is string)
                {
                    Obj.SetPropertyValue(PropertyName, SetValue, CaseCompare);
                }
                else if (SetValue.TryGetPropertyValue(PropertyName, out var GetValue, CaseCompare))
                {
                    Obj.SetPropertyValue(PropertyName, GetValue, CaseCompare);
                }
            }
            return Obj;
        }

        public static TModel SetPropertyValue<TModel, TValue>(this TModel Obj, TValue SetValue, CaseCompareType CaseCompare = CaseCompareType.None) where TValue : new()
        {
            var AllProperty = SetValue.GetType().GetProperties();
            foreach (var Item in AllProperty)
            {
                var PropertyName = Item.Name;
                if (SetValue.TryGetPropertyValue(PropertyName, out var GetValue, CaseCompare))
                {
                    Obj.SetPropertyValue(PropertyName, GetValue, CaseCompare);
                }
            }
            return Obj;
        }

        public static TModel SetPropertyValueIfNotNull<TModel, TValue>(this TModel Obj, TValue SetValue, CaseCompareType CaseCompare = CaseCompareType.None) where TValue : new()
        {
            var AllProperty = SetValue.GetType().GetProperties();
            foreach (var Item in AllProperty)
            {
                var PropertyName = Item.Name;
                if (SetValue.TryGetPropertyValue(PropertyName, out var GetValue, CaseCompare) && GetValue is not null)
                {
                    Obj.SetPropertyValue(PropertyName, GetValue, CaseCompare);
                }
            }
            return Obj;
        }


        public static bool IsHasProperty<TModel>(this TModel Obj, string PropertyName, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            var AllProperty = Obj.GetType().GetProperties();
            var IsHas = AllProperty
                .Any(Item =>
                {
                    var GetName = Item.Name;
                    var SetName = PropertyName;
                    if (CaseCompare == CaseCompareType.None)
                    {
                        GetName = GetName.ToLower();
                        SetName = SetName.ToLower();
                    }
                    return GetName == SetName;
                });

            return IsHas;
        }
    }
}
