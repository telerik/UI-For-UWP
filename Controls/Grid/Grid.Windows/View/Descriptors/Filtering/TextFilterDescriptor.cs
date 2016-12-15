using System;
using System.Diagnostics;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a <see cref="PropertyFilterDescriptor"/> that is associated with the <see cref="System.String"/> data type.
    /// </summary>
    public class TextFilterDescriptor : PropertyFilterDescriptor
    {
        private TextOperator textOperator;
        private bool isCaseSensitive;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFilterDescriptor" /> class.
        /// </summary>
        public TextFilterDescriptor()
        {
            this.isCaseSensitive = true;
        }

        /// <summary>
        /// Gets or sets the <see cref="TextOperator"/> value that defines how the <see cref="P:Value"/> member is compared with each value from the items source.
        /// </summary>
        public TextOperator Operator
        {
            get
            {
                return this.textOperator;
            }
            set
            {
                if (this.textOperator == value)
                {
                    return;
                }

                this.textOperator = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the text comparisons will be case-sensitive. Defaults to true.
        /// </summary>
        public bool IsCaseSensitive
        {
            get
            {
                return this.isCaseSensitive;
            }
            set
            {
                if (this.isCaseSensitive == value)
                {
                    return;
                }

                this.isCaseSensitive = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Prepares Where clause for current text filter.
        /// </summary>
        /// <returns></returns>
        internal override string SerializeToSQLiteWhereCondition()
        {
            string condition = string.Empty;
            var filterStringRepresentation = string.Empty;

            if (this.Value != null)
            {
                filterStringRepresentation = this.Value.ToString();
            }

            if (!this.isCaseSensitive)
            {
                filterStringRepresentation = filterStringRepresentation.ToLower();
            }
         
            switch (this.textOperator)
            {
                case TextOperator.Contains:
                    condition = this.PropertyName + " LIKE '%" + filterStringRepresentation + "%'"; 
                    break;
                case TextOperator.DoesNotContain:
                    condition = this.PropertyName + " NOT LIKE '%" + filterStringRepresentation + "%'"; 
                    break;
                case TextOperator.EqualsTo:
                    condition = this.PropertyName + " LIKE '" + filterStringRepresentation + "'"; 
                    break;
                case TextOperator.DoesNotEqualTo:
                    condition = this.PropertyName + " NOT LIKE '" + filterStringRepresentation + "'"; 
                    break;
                case TextOperator.StartsWith:
                    condition = this.PropertyName + " LIKE '" + filterStringRepresentation + "%'"; 
                    break;
                case TextOperator.EndsWith:
                    condition = this.PropertyName + " LIKE '%" + filterStringRepresentation + "'"; 
                    break;
            }

            return condition;
        }

        /// <summary>
        /// Encapsulates the core filter logic exposed by the descriptor. Allows inheritors to provide their own custom filtering logic.
        /// </summary>
        /// <param name="itemValue">The property value, as defined by the <see cref="P:PropertyName" /> property.</param>
        /// <returns>
        /// True if the filter is passed and the associated item should be displayed, false otherwise.
        /// </returns>
        protected override bool PassesFilterOverride(object itemValue)
        {
            if (itemValue == null || this.Value == null)
            {
                return itemValue == this.Value;
            }

            var itemStringRepresentation = itemValue.ToString();

            var filterValue = this.Value;
            var filterStringRepresentation = string.Empty;
            if (filterValue != null)
            {
                filterStringRepresentation = filterValue.ToString();
            }

            if (!this.isCaseSensitive)
            {
                itemStringRepresentation = itemStringRepresentation.ToLower();
                filterStringRepresentation = filterStringRepresentation.ToLower();
            }

            bool passes = true;

            switch (this.textOperator)
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