using System;

namespace Telerik.Data.Core
{
   /// <summary>
    /// Represents a <see cref="PropertyFilterDescriptor"/> that is associated with the objects that have properties.
    /// </summary>
    public class DataGridNestedPropertyTextFilterDescriptor : TextFilterDescriptor
    {
        /// <summary>
        /// Gets or sets the item property getter.
        /// </summary>
        /// <value>The item property getter.</value>
        public Func<object, object> ItemPropertyGetter { get; set; }

        internal override string SerializeToSQLiteWhereCondition()
        {
            throw new NotSupportedException("Serialization is not supported for nested properties.");
        }

        /// <inheritdoc/>
        protected override bool PassesFilterOverride(object itemValue)
        {
            if (itemValue == null || this.Value == null)
            {
                return itemValue == this.Value;
            }

            string itemStringRepresentation;
            if (this.ItemPropertyGetter != null)
            {
                itemStringRepresentation = this.ItemPropertyGetter(itemValue).ToString();
            }
            else
            {
                itemStringRepresentation = itemValue.ToString();
            }

            var filterValue = this.Value;
            var filterStringRepresentation = string.Empty;
            if (filterValue != null)
            {
                filterStringRepresentation = filterValue.ToString();
            }

            if (!this.IsCaseSensitive)
            {
                itemStringRepresentation = itemStringRepresentation.ToLower();
                filterStringRepresentation = filterStringRepresentation.ToLower();
            }

            bool passes = true;

            switch (this.Operator)
            {
                case TextOperator.Contains:
                    passes = itemStringRepresentation.Contains(filterStringRepresentation);
                    break;
                case TextOperator.DoesNotContain:
                    passes = !itemStringRepresentation.Contains(filterStringRepresentation);
                    break;
                case TextOperator.EqualsTo:
                    passes = itemStringRepresentation.Equals(filterStringRepresentation);
                    break;
                case TextOperator.DoesNotEqualTo:
                    passes = !itemStringRepresentation.Equals(filterStringRepresentation);
                    break;
                case TextOperator.StartsWith:
                    passes = itemStringRepresentation.StartsWith(filterStringRepresentation, StringComparison.CurrentCulture);
                    break;
                case TextOperator.EndsWith:
                    passes = itemStringRepresentation.EndsWith(filterStringRepresentation, StringComparison.CurrentCulture);
                    break;
            }

            return passes;
        }
    }
}