using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    public class EnvironmentElement : ConfigurationElement
    {
        [ConfigurationProperty(Constants.VariablesName)]
        public VariableCollection Variables
          => (VariableCollection)this[Constants.VariablesName];

        [ConfigurationProperty(Constants.NameName, IsRequired = true)]
        public string Name => (string) base[Constants.NameName];
    }
}