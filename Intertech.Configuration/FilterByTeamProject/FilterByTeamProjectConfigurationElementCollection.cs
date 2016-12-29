using System;
using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.FilterByTeamProject
{
    [ConfigurationCollection(typeof(FilterByTeamProjectConfigurationElement), AddItemName = Constants.FilterByTeamProjectName, CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class FilterByTeamProjectConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FilterByTeamProjectConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return ((FilterByTeamProjectConfigurationElement)element).TeamProjectName;
        }

        [ConfigurationProperty(Constants.EnabledName, DefaultValue = false)]
        public bool Enabled
        {
            get
            {
                return (bool)base[Constants.EnabledName];
            }
        }
    }
}
