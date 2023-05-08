using Rugal.Net.DynamicSetter.Extention;
using System.Linq.Expressions;

namespace Rugal.Net.DynamicSetter.Model
{
    public class ModelSetter<TModel> where TModel : class
    {
        public TModel Model;
        private object FromModel;
        public ModelSetter(TModel _Model)
        {
            Model = _Model;
        }
        public ModelSetter(TModel _Model, object _FromModel) : this(_Model)
        {
            FromModel = _FromModel;
        }
        public ModelSetter<TModel> With(object _FromModel)
        {
            FromModel = _FromModel;
            return this;
        }

        public ModelSetter<TModel> Set(Action<TModel> SetAction)
        {
            SetAction.Invoke(Model);
            return this;
        }
        public ModelSetter<TModel> Set<TValue>(Expression<Func<TModel, TValue>> Exp, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            Model.SetPropertyWith(FromModel, Exp, CaseCompare);
            return this;
        }
        public ModelSetter<TModel> Set<TGetValue, TSetValue>(Expression<Func<TModel, TGetValue>> Exp, TSetValue SetValue, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            Model.SetPropertyValue(Exp, SetValue, CaseCompare);
            return this;
        }
        public ModelSetter<TModel> Set<TValue>(TValue SetValue, CaseCompareType CaseCompare = CaseCompareType.None) where TValue : new()
        {
            Model.SetPropertyValue(SetValue, CaseCompare);
            return this;
        }

        public ModelSetter<TModel> SetIfNotNull<TValue>(Expression<Func<TModel, TValue>> Exp, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            Model.SetPropertyWith_NotNull(FromModel, Exp, CaseCompare);
            return this;
        }
        public ModelSetter<TModel> SetIfNotNull<TValue>(TValue SetValue, CaseCompareType CaseCompare = CaseCompareType.None) where TValue : new()
        {
            Model.SetPropertyValueIfNotNull(SetValue, CaseCompare);
            return this;
        }
        public ModelSetter<TModel> AutoSet(CaseCompareType CaseCompare = CaseCompareType.None)
        {
            var AllFromProperty = FromModel.GetType().GetProperties();
            var AllSetProperty = Model.GetType().GetProperties();
            foreach (var FromProperty in AllFromProperty)
            {
                var GetName = FromProperty.Name;
                if (CaseCompare == CaseCompareType.None)
                    GetName = GetName.ToLower();

                var GetProperty = AllSetProperty
                    .Where(Item =>
                    {
                        var SetName = Item.Name;
                        if (CaseCompare == CaseCompareType.None)
                            SetName = SetName.ToLower();
                        return GetName == SetName;
                    })
                    .FirstOrDefault();

                if (GetProperty is null)
                    continue;

                var SetValue = FromProperty.GetValue(FromModel);
                GetProperty.SetValue(Model, SetValue);
            }
            return this;
        }
        public ModelSetter<TModel> AutoSetIfNotNull(CaseCompareType CaseCompare = CaseCompareType.None)
        {
            var AllFromProperty = FromModel.GetType().GetProperties();
            var AllSetProperty = Model.GetType().GetProperties();
            foreach (var FromProperty in AllFromProperty)
            {
                var GetName = FromProperty.Name;
                if (CaseCompare == CaseCompareType.None)
                    GetName = GetName.ToLower();

                var GetProperty = AllSetProperty
                    .Where(Item =>
                    {
                        var SetName = Item.Name;
                        if (CaseCompare == CaseCompareType.None)
                            SetName = SetName.ToLower();
                        return GetName == SetName;
                    })
                    .FirstOrDefault();

                if (GetProperty is null)
                    continue;

                var SetValue = FromProperty.GetValue(FromModel);
                if (SetValue is null)
                    continue;

                GetProperty.SetValue(Model, SetValue);
            }
            return this;
        }
        public ModelSetter<TModel> AutoSet(object _FromModel, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            FromModel = _FromModel;
            AutoSet(CaseCompare);
            return this;
        }
        public ModelSetter<TModel> AutoSetIfNotNull(object _FromModel, CaseCompareType CaseCompare = CaseCompareType.None)
        {
            FromModel = _FromModel;
            AutoSetIfNotNull(CaseCompare);
            return this;
        }

        public static ModelSetter<TModel> Create(TModel Model)
        {
            var Setter = new ModelSetter<TModel>(Model);
            return Setter;
        }
        public static ModelSetter<TModel> Create(TModel Model, object FromModel)
        {
            var Setter = new ModelSetter<TModel>(Model, FromModel);
            return Setter;
        }
    }
}
