using System.Configuration;
using Intertech.Configuration.FilterByTeamProject;
using Intertech.Configuration.ProgramTypeTemplate;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration
{
    public class PluginConfigurationSection : ConfigurationSection
    {

        [ConfigurationProperty(Constants.ProgramTypeTemplatesName)]
        public ProgramTypeTemplateCollection ProgramTypeTemplates
            => (ProgramTypeTemplateCollection) this[Constants.ProgramTypeTemplatesName];

        [ConfigurationProperty(Constants.FilterByTeamProjectsName)]
        public FilterByTeamProjectConfigurationElementCollection FilterByTeamProjects
            => (FilterByTeamProjectConfigurationElementCollection)this[Constants.FilterByTeamProjectsName];

    }
}