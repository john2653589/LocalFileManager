using Rugal.Net.LocalFileManager.Model;
using Rugal.Net.LocalFileManager.Service;

namespace Rugal.Net.LocalFileManager.Extention
{
    public static class StartupExtention
    {
        public static IServiceCollection AddLocalFile(this IServiceCollection Services, IConfiguration Configuration, string ConfigurationKey = "LocalFile")
        {
            var Setting = NewSetting(Configuration, ConfigurationKey);
            AddLocalFileSetting(Services, Setting);
            AddLocalFileService(Services);

            return Services;
        }
        public static IServiceCollection AddLocalFile(this IServiceCollection Services, IConfiguration Configuration,
           Action<LocalFileManagerSetting, IServiceProvider> SettingFunc, string ConfigurationKey = "LocalFile")
        {
            Services.AddLocalFile(Configuration, ConfigurationKey, SettingFunc);
            return Services;
        }
        public static IServiceCollection AddLocalFile(this IServiceCollection Services, IConfiguration Configuration,
            string ConfigurationKey, Action<LocalFileManagerSetting, IServiceProvider> SettingFunc)
        {
            var Setting = NewSetting(Configuration, ConfigurationKey);
            AddLocalFileSetting(Services, Setting, SettingFunc);
            AddLocalFileService(Services);
            return Services;
        }

        public static IServiceCollection AddLocalFile(this IServiceCollection Services, string RootPath, string RemoteDomain = null)
        {
            AddLocalFileSetting(Services, RootPath, RemoteDomain);
            AddLocalFileService(Services);
            return Services;
        }
        public static IServiceCollection AddLocalFile(this IServiceCollection Services,
            string RootPath, string RemoteDomain, Action<LocalFileManagerSetting, IServiceProvider> SettingFunc)
        {
            AddLocalFileSetting(Services, RootPath, RemoteDomain, SettingFunc);
            AddLocalFileService(Services);
            return Services;
        }

        public static IServiceCollection AddLocalFileSetting(this IServiceCollection Services, string RootPath, string RemoteDomain = null)
        {
            var Setting = new LocalFileManagerSetting()
            {
                RootPath = RootPath,
                RemoteDomain = RemoteDomain,
            };
            Services.AddSingleton(Setting);
            return Services;
        }
        public static IServiceCollection AddLocalFileSetting(this IServiceCollection Services, LocalFileManagerSetting Setting)
        {
            Services.AddSingleton(Setting);
            return Services;
        }
        public static IServiceCollection AddLocalFileSetting(this IServiceCollection Services,
            string RootPath, string RemoteDomain, Action<LocalFileManagerSetting, IServiceProvider> SettingFunc)
        {
            Services.AddScoped((Provider) =>
            {
                var Setting = new LocalFileManagerSetting()
                {
                    RootPath = RootPath,
                    RemoteDomain = RemoteDomain,
                };
                SettingFunc?.Invoke(Setting, Provider);
                return Setting;
            });
            return Services;
        }
        public static IServiceCollection AddLocalFileSetting(this IServiceCollection Services, LocalFileManagerSetting Setting, Action<LocalFileManagerSetting, IServiceProvider> SettingFunc)
        {
            Services.AddScoped((Provider) =>
            {
                SettingFunc.Invoke(Setting, Provider);
                return Setting;
            });
            return Services;
        }
        public static IServiceCollection AddLocalFileService(this IServiceCollection Services)
        {
            Services.AddScoped<LocalFileService>();
            return Services;
        }

        private static LocalFileManagerSetting NewSetting(IConfiguration Configuration, string ConfigurationKey)
        {
            var GetSetting = Configuration.GetSection(ConfigurationKey);
            var Spm = GetSetting.GetValue<string>("Spm");
            var Setting = new LocalFileManagerSetting()
            {
                RootPath = GetSetting.GetValue<string>("RootPath"),
                RemoteDomain = GetSetting.GetValue<string>("RemoteDomain"),
                SyncPerMin = Spm == null ? null : TimeSpan.FromMinutes(int.Parse(Spm)),
            };
            return Setting;
        }
    }
}
