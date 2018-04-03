using System.Reflection;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a GridFormEntityProperty entity property.
    /// </summary>
    public class GridFormEntityProperty : RuntimeEntityProperty
    {
        private DataGridColumn column;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridFormEntityProperty"/> class.
        /// </summary>
        public GridFormEntityProperty(PropertyInfo property, object item, DataGridColumn column)
        : base(property, item)
        {
            this.column = column;
        }

        /// <summary>
        /// Gets or sets a path to a value on the source object to provide the visual representation of the object. 
        /// </summary>
        public string DisplayMemberPath { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the path used to get the value. 
        /// </summary>
        public string SelectedValuePath { get; set; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override bool GetIsReadOnly(object property)
        {
            if (this.column != null && !this.column.CanEdit)
            {
                return true;
            }

            return base.GetIsReadOnly(property);
        }
    }
}
