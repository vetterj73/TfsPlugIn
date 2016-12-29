using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.FilterByTeamProject
{
    public class FilterByTeamProjectConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(Constants.TeamProjectName, IsRequired = true)]
        public string TeamProjectName => (string)base[Constants.TeamProjectName];

        [ConfigurationProperty(Constants.EnabledName, DefaultValue = false)]
        public bool Enabled => (bool)base[Constants.EnabledName];

    }
}