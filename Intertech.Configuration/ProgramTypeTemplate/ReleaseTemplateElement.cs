using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    public class ReleaseTemplateElement : BaseTemplateElement
    {
        [ConfigurationProperty(Constants.EnviromentsName, IsRequired = false)]
        public EnvironmentCollection Environments
         => (EnvironmentCollection)this[Constants.EnviromentsName];

    }
}