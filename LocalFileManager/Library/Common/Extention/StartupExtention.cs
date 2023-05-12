using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rugal.NetCommon.Extention.JsonConvert;
using Rugal.NetCommon.FormDataConverts;
using System.Text.Json.Serialization;

namespace Rugal.NetCommon.Extention.Startup
{
    public static class StartupExtention
    {
        public static IServiceCollection AddCommonJsonOption(this IServiceCollection Services)
        {
            Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                options.JsonSerializerOptions.Converters.AddAllConvert();
            });
            return Services;
        }
        public static IServiceCollection AddCommonFormDataOption(this IServiceCollection Services)
        {
            Services.AddControllers(options =>
            {
                options.ValueProviderFactories
                   .Where(Item => Item is FormValueProviderFactory || Item is JQueryFormValueProviderFactory)
                   .ToList()
                   .ForEach(Item => options.ValueProviderFactories.Remove(Item));
                options.ValueProviderFactories.Insert(0, new CanNullFormValueProviderFactory());
            });
            return Services;
        }
        public static IServiceCollection AddCommonInputOptions(this IServiceCollection Services)
        {
            Services.AddCommonJsonOption();
            Services.AddCommonFormDataOption();
            return Services;
        }
        public static IServiceCollection AddCorsAll(this IServiceCollection Services, ConfigurationManager Configuration, IWebHostEnvironment Evn)
        {
            Services.AddCors(options =>
            {
                var GetCorsOrigin = Configuration["CorsOrigin"];
                if (Evn.IsDevelopment())
                {
                    options.AddDefaultPolicy(builder =>
                    {
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowCredentials();
                        builder.SetIsOriginAllowed(origin =>
                        {
                            var IsAllowed = new Uri(origin).Host.ToLower().Contains("localhost");
                            return IsAllowed;
                        });
                    });
                }
                else if (!string.IsNullOrWhiteSpace(GetCorsOrigin))
                {
                    var AllCorsOrigin = GetCorsOrigin.Split(',');
                    options.AddPolicy("CorsAll", builder =>
                    {
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowCredentials();
                        builder.WithOrigins(AllCorsOrigin);
                    });
                }
            });
            return Services;
        }

        public static void UseCorsAll(this IApplicationBuilder App, IWebHostEnvironment Evn)
        {
            if (Evn.IsDevelopment())
                App.UseCors();
            else
                App.UseCors("CorsAll");
        }
        public static void UseCorsAny(this IApplicationBuilder App, IWebHostEnvironment Evn)
        {
            App.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
            });
        }
    }
}