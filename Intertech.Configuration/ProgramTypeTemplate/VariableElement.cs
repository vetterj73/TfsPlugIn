using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    public class VariableElement : ConfigurationElement
    {
        [ConfigurationProperty(Constants.NameName, IsRequired = true)]
        public string Name => (string)base[Constants.NameName];

        [ConfigurationProperty(Constants.VariableTypeName, IsRequired = false)]
        public string VariableType => (string)base[Constants.VariableTypeName];

        [ConfigurationProperty(Constants.VariablePropName, IsRequired = false)]
        public string VariableProp => (string)base[Constants.VariablePropName];

        [ConfigurationProperty(Constants.VariableValueName, IsRequired = false)]
        public string VariableValue => (string)base[Constants.VariableValueName];
    }
}