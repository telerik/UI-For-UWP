using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public class GridFormEntityProperty : RuntimeEntityProperty
    {
        public GridFormEntityProperty(PropertyInfo property, object item)
        : base(property, item)
    {
    }

        protected override string GetLabel(object property)
        {
            var label = base.GetLabel(property);
            if (label == null)
            {
                var propertyInfo = property as PropertyInfo;
                if (propertyInfo != null)
                {
                    return propertyInfo.Name;
                }
            }

            return label;
        }

        public string DisplayMemberPath { get; set; }
        public string SelectedValuePath { get; set; }
    }
}
