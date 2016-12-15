using System;
using System.Diagnostics;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a <see cref="PropertyFilterDescriptor"/> that is associated with the <see cref="System.Boolean"/> data type.
    /// </summary>
    public class BooleanFilterDescriptor : PropertyFilterDescriptor
    {
        private bool? convertedValue;

        /// <summary>
        /// Provides Where clause for boolean filter.
        /// </summary>
        /// <returns></returns>
        internal override string SerializeToSQLiteWhereCondition()
        {
            string result = this.PropertyName + " == " + (this.Value.ToString() == "True" ? '1' : '0');
            return result;
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

            if (this.convertedValue == null)
            {
                bool value;
                if (this.Value is string)
                {
                    if (bool.TryParse((string)this.Value, out value))
                    {
                        this.convertedValue = value;
                    }
                }
                else if (TryGetBooleanFromValue(this.Value, out value))
                {
                    this.convertedValue = value;
                }
            }

            if (!this.convertedValue.HasValue)
            {
                return false;
            }

            bool itemBoolValue;
            if (!TryGetBooleanFromValue(itemValue, out itemBoolValue))
            {
                return false;
            }

            return itemBoolValue == this.convertedValue.Value;
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        /// <param name="changedPropertyName"></param>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (changedPropertyName == "Value")
            {
                this.convertedValue = null;
            }

            base.PropertyChangedOverride(changedPropertyName);
        }

        private static bool TryGetBooleanFromValue(object value, out bool result)
        {
            result = false;
            if (value is bool)
            {
                result = (bool)value;
                return true;
            }

            if (value is bool?)
            {
                var nullable = (bool?)value;
                if (nullable.HasValue)
                {
                    result = nullable.Value;
                    return true;
                }
            }

            return false;
        }
    }
}
