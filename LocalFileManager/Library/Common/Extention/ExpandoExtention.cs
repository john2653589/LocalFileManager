using System.Dynamic;

namespace Rugal.NetCommon.Extention.Expando
{
    /// <summary>
    /// v1.1
    /// </summary>
    public static class ExpandoExtention
    {
        public static ExpandoObject ToExpando(this object Model)
        {
            var Ret = new ExpandoObject();
            if (Model is null)
                return Ret;

            if (Model is ExpandoObject)
                return Model as ExpandoObject;

            var AllProperty = Model.GetType().GetProperties();
            foreach (var Property in AllProperty)
            {
                var GetValue = Property.GetValue(Model);
                Ret.TryAdd(Property.Name, GetValue);
            }
            return Ret;
        }
        public static ExpandoObject ToExpando<TKey, TValue>(this Dictionary<TKey, TValue> Dic)
        {
            var Ret = new ExpandoObject();
            foreach (var Item in Dic)
                Ret.TryAdd(Item.Key.ToString(), Item.Value);
            return Ret;
        }
        public static object Extend(this object Model, object AddModel, bool IsReplace = false)
        {
            var Ret = Model.ToExpando();
            Extned(Ret, AddModel, IsReplace);
            return Ret;
        }
        public static object Extend<TKey, TValue>(this object Model, Dictionary<TKey, TValue> AddDic, bool IsReplace = false)
        {
            var Ret = Model.ToExpando();
            Extned(Ret, AddDic, IsReplace);
            return Ret;
        }
        public static void Extned(this ExpandoObject Model, object AddModel, bool IsReplace = false)
        {
            var AddModelExpando = AddModel.ToExpando();
            foreach (var Item in AddModelExpando)
            {
                var Key = Item.Key;
                var Value = Item.Value;
                if (!Model.TryAdd(Key, Value) && IsReplace)
                {
                    var Dic = Model as IDictionary<string, object>;
                    Dic[Key] = Value;
                }
            }
        }
        public static void Extned<TKey, TValue>(this ExpandoObject Model, Dictionary<TKey, TValue> AddDic, bool IsReplace = false)
        {
            foreach (var Item in AddDic)
            {
                var Key = Item.Key.ToString();
                var Value = Item.Value;
                if (!Model.TryAdd(Key, Value) && IsReplace)
                {
                    var Dic = Model as IDictionary<string, object>;
                    Dic[Key] = Value;
                }
            }
        }
    }
}