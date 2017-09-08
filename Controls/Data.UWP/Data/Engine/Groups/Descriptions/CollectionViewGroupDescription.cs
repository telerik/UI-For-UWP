using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Telerik.Data.Core
{
    internal class CollectionViewGroupDescription : PropertyGroupDescriptionBase
    {
        protected internal override object GroupNameFromItem(object item, int level)
        {
            return null;
        }

        /// <inheritdoc />
        protected override Cloneable CreateInstanceCore()
        {
            return new CollectionViewGroupDescription();
        }

        /// <inheritdoc />
        protected override void CloneOverride(Cloneable source)
        {
        }
    }
}
