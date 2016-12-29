using System;
using System.Collections.Generic;
using System.Configuration;
using Intertech.Tfs.Common.Utilities;

namespace Intertech.Configuration.ProgramTypeTemplate
{
    [ConfigurationCollection(typeof(ProgramTypeTemplateElement), AddItemName = Constants.ProgramTypeemplateName, CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ProgramTypeTemplateCollection : ConfigurationElementCollection, IEnumerable<ProgramTypeTemplateElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProgramTypeTemplateElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return ((ProgramTypeTemplateElement)element).ProgramTypePathStartsWith;
        }

        public new IEnumerator<ProgramTypeTemplateElement> GetEnumerator()
        {
            foreach (var key in BaseGetAllKeys())
            {
                yield return (ProgramTypeTemplateElement)BaseGet(key);
            }
        }

        [ConfigurationProperty(Constants.EnabledName, IsRequired = true)]
        public bool Enabled => (bool)base[Constants.EnabledName];

    }
}