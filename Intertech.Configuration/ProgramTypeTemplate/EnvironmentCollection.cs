using System;
using System.Collections.Generic;
using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    [ConfigurationCollection(typeof(EnvironmentElement), AddItemName = Constants.EnviromentName, CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class EnvironmentCollection : ConfigurationElementCollection, IEnumerable<EnvironmentElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return ((EnvironmentElement)element).Name;
        }

        public new IEnumerator<EnvironmentElement> GetEnumerator()
        {
            foreach (var key in BaseGetAllKeys())
            {
                yield return (EnvironmentElement)BaseGet(key);
            }
        }

    }
}