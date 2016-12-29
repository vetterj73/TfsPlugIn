using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    public class ProgramTypeTemplateElement : ConfigurationElement
    {
        [ConfigurationProperty(Constants.EnabledName, IsRequired = true)]
        public bool Enabled => (bool)base[Constants.EnabledName];

        [ConfigurationProperty(Constants.ProgramTypePathStartsWithName, IsRequired = true)]
        public string ProgramTypePathStartsWith => (string)base[Constants.ProgramTypePathStartsWithName];

        [ConfigurationProperty(Constants.BuildTemplateName, IsRequired = true)]
        public BuildTemplateElement BuildTemplate => (BuildTemplateElement)base[Constants.BuildTemplateName];

        [ConfigurationProperty(Constants.ReleaseTemplateName, IsRequired = true)]
        public ReleaseTemplateElement ReleaseTemplate => (ReleaseTemplateElement)base[Constants.ReleaseTemplateName];


    }
}