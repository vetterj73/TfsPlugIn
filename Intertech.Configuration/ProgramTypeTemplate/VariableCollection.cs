using System;
using System.Collections.Generic;
using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    [ConfigurationCollection(typeof(VariableElement), AddItemName = Constants.VariableName, CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class VariableCollection : ConfigurationElementCollection, IEnumerable<VariableElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new VariableElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return ((VariableElement)element).Name;
        }

        public new IEnumerator<VariableElement> GetEnumerator()
        {
            foreach (var key in BaseGetAllKeys())
            {
                yield return (VariableElement)BaseGet(key);
            }
        }
    }
}