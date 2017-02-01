using System;
using System.Diagnostics;
using System.Globalization;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a <see cref="PropertyFilterDescriptor"/> that is associated with all the numerical data types available in .NET.
    /// <remarks>
    /// The known numerical types are: <c>byte</c>, <c>sbyte</c>, <c>short</c>, <c>ushort</c>, <c>int</c>, <c>uint</c>,
    /// <c>long</c>, <c>ulong</c>, <c>float</c>, <c>double</c>, <c>decimal</c>.
    /// </remarks>
    /// </summary>
    public class NumericalFilterDescriptor : PropertyFilterDescriptor
    {
        private NumericalOperator numericalOperator;
        private double? convertedValue;

        /// <summary>
        /// Gets or sets the <see cref="NumericalOperator"/> value that defines the boolean logic behind the left and right operand comparison.
        /// </summary>
        public NumericalOperator Operator
        {
            get
            {
                return this.numericalOperator;
            }
            set
            {
                if (this.numericalOperator == value)
                {
                    return;
                }

                this.numericalOperator = value;
                this.OnPropertyChanged();
            }
        }

        internal double? ConvertedValue
        {
            get
            {
                if (this.convertedValue == null)
                {
                    this.TryConvertValue();
                }

                return this.convertedValue;
            }
        }

        /// <summary>
        /// Prepares Where clause for current numerical filter.
        /// </summary>
        /// <returns></returns>
        internal override string SerializeToSQLiteWhereCondition()
        {
            if (this.convertedValue == null)
            {
                this.TryConvertValue();
            }

            if (!this.convertedValue.HasValue)
            {
                return string.Empty;
            }

            string condition = string.Empty;
            string convertedValueString = this.convertedValue.Value.ToString(NumberFormatInfo.InvariantInfo);

            switch (this.numericalOperator)
            {
                case NumericalOperator.EqualsTo:
                    condition = this.PropertyName + " == " + convertedValueString;
                    break;
                case NumericalOperator.DoesNotEqualTo:
                    condition = this.PropertyName + " != " + convertedValueString;
                    break;
                case NumericalOperator.IsGreaterThan:
                    condition = this.PropertyName + " > " + convertedValueString;
                    break;
                case NumericalOperator.IsGreaterThanOrEqualTo:
                    condition = this.PropertyName + " >= " + convertedValueString;
                    break;
                case NumericalOperator.IsLessThan:
                    condition = this.PropertyName + " < " + convertedValueString;
                    break;
                case NumericalOperator.IsLessThanOrEqualTo:
                    condition = this.PropertyName + " <= " + convertedValueString;
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

            if (this.convertedValue == null)
            {
                this.TryConvertValue();
            }

            if (!this.convertedValue.HasValue)
            {
                return false;
            }

            double itemDouble;
            if (!NumericConverter.TryConvertToDouble(itemValue, out itemDouble, CultureInfo.InvariantCulture, true))
            {
                return false;
            }

            bool passes = true;

            switch (this.numericalOperator)
            {
                case NumericalOperator.EqualsTo:
                    passes = this.convertedValue.Value == itemDouble;
                    break;
                case NumericalOperator.DoesNotEqualTo:
                    passes = this.convertedValue.Value != itemDouble;
                    break;
                case NumericalOperator.IsGreaterThan:
                    passes = itemDouble > this.convertedValue.Value;
                    break;
                case NumericalOperator.IsGreaterThanOrEqualTo:
                    passes = itemDouble >= this.convertedValue.Value;
                    break;
                case NumericalOperator.IsLessThan:
                    passes = itemDouble < this.convertedValue.Value;
                    break;
                case NumericalOperator.IsLessThanOrEqualTo:
                    passes = itemDouble <= this.convertedValue.Value;
                    break;
            }

            return passes;
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

        private void TryConvertValue()
        {
            if (this.Value == null)
            {
                return;
            }

            double value;

            // try parse the value from string first - XAML support
            if (this.Value is string)
            {
                if (double.TryParse((string)this.Value, out value))
                {
                    this.convertedValue = value;
                }
            }
            else if (NumericConverter.TryConvertToDouble(this.Value, out value, CultureInfo.InvariantCulture, true))
            {
                this.convertedValue = value;
            }
        }
    }
}
