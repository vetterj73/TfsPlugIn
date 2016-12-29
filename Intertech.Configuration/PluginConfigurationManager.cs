using System;
using System.Configuration;
using System.Reflection;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration
{
    public class PluginConfigurationManager
    {
        private static Assembly configurationDefiningAssembly;

        static PluginConfigurationManager()
        {
            string strPluginFile = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            configurationDefiningAssembly = Assembly.LoadFrom(strPluginFile);
            var exeFileMap = new ExeConfigurationFileMap();
            exeFileMap.ExeConfigFilename = $"{strPluginFile}.config";
            var customConfig = ConfigurationManager.OpenMappedExeConfiguration(exeFileMap, ConfigurationUserLevel.None);

            // This handler is needed so that the GetSection is able to retrieve the type since this dll isn't in the GAC
            AppDomain.CurrentDomain.AssemblyResolve += ConfigResolveEventHandler;
            Section = customConfig.GetSection(Constants.PluginConfigurationSectionName) as PluginConfigurationSection;
            AppDomain.CurrentDomain.AssemblyResolve -= ConfigResolveEventHandler;
        }

        public static PluginConfigurationSection Section { get; private set; }

        private static Assembly ConfigResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return configurationDefiningAssembly;
        }
    }
}
