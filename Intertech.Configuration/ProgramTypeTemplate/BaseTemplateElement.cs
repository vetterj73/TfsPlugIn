using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    public abstract class BaseTemplateElement : ConfigurationElement
    {
        [ConfigurationProperty(Constants.VariablesName)]
        public VariableCollection Variables
          => (VariableCollection)this[Constants.VariablesName];

        [ConfigurationProperty(Constants.TemplateNameName, IsRequired = true)]
        public string TemplateName => (string)base[Constants.TemplateNameName];
    }
}