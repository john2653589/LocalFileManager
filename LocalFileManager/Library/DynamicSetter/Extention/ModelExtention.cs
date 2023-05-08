using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rugal.Net.DynamicSetter.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace Rugal.Net.DynamicSetter.Extention
{
    public static class ModelExtention
    {
        public static ModelSetter<TModel> ToSetter<TModel>(this TModel Model) where TModel : class
        {
            var Setter = ModelSetter<TModel>.Create(Model);
            return Setter;
        }
        public static ModelSetter<TModel> ToSetter<TModel>(this TModel Model, object FromModel) where TModel : class
        {
            var Setter = ModelSetter<TModel>.Create(Model, FromModel);
            return Setter;
        }

        public static TModel SetPropertyWith<TModel, TValue>(this TModel SetModel, object WithModel, Expression<Func<TModel, TValue>> PropertyExp, CaseCompareType CaseCompare = CaseCompareType.None) where TModel : class
        {
            var Members = PropertyExp.GetMembers();
            foreach (var Item in Members)
            {
                var PropertyName = Item.Name;
                if (!WithModel.TryGetPropertyValue(PropertyName, out var GetValue, CaseCompare))
                    continue;

                SetModel.SetPropertyValue(PropertyName, GetValue, CaseCompare);
            }
            return SetModel;
        }

        public static TModel SetPropertyWith_NotNull<TModel, TValue>(this TModel SetModel, object WithModel, Expression<Func<TModel, TValue>> PropertyExp, CaseCompareType CaseCompare = CaseCompareType.None) where TModel : class
        {
            var Members = PropertyExp.GetMembers();
            foreach (var Item in Members)
            {
                var PropertyName = Item.Name;
                if (!WithModel.TryGetPropertyValue(PropertyName, out var GetValue, CaseCompare))
                    continue;

                if (GetValue is null)
                    continue;

                SetModel.SetPropertyValue(PropertyName, GetValue, CaseCompare);
            }

            return SetModel;
        }
    }
}
