using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Rugal.NetCommon.FormDataConverts
{
    public class CanNullFormValueProvider : FormValueProvider
    {
        public CanNullFormValueProvider(BindingSource bindingSource, IFormCollection values, CultureInfo culture) : base(bindingSource, values, culture)
        {
        }
        public override ValueProviderResult GetValue(string key)
        {
            var Value = base.GetValue(key);
            var EmptyValue = new[] { "null", "", @"""""", "''" }.Select(Item => new ValueProviderResult(Item));
            if (EmptyValue.Any(Item => Item == Value))
            {
                var None = ValueProviderResult.None;
                return None;
            }
            return Value;
        }
    }
    public class CanNullFormValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            var HttpContext = context.ActionContext.HttpContext;
            if (HttpContext.Request.HasFormContentType)
            {
                var FormValueProvider = new CanNullFormValueProvider(
                    BindingSource.Form,
                    HttpContext.Request.Form,
                    CultureInfo.InvariantCulture);
                context.ValueProviders.Add(FormValueProvider);
            }
            return Task.CompletedTask;
        }
    }
}
