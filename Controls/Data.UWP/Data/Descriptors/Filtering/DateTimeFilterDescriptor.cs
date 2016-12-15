using System;
using System.Globalization;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a <see cref="PropertyFilterDescriptor"/> that is associated with the <see cref="System.DateTime"/> data type.
    /// </summary>
    public class DateTimeFilterDescriptor : PropertyFilterDescriptor
    {
        private NumericalOperator filterOperator;
        private DateTime? convertedValue;
        private DateTimePart part;

        /// <summary>
        /// Gets or sets the <see cref="NumericalOperator"/> value that defines the boolean logic behind the left and right operand comparison.
        /// </summary>
        public NumericalOperator Operator
        {
            get
            {
                return this.filterOperator;
            }
            set
            {
                if (this.filterOperator == value)
                {
                    return;
                }

                this.filterOperator = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTimePart"/> value that determines which parts of the underlying DateTime structures participate in the filtering comparison.
        /// Defaults to <see cref="DateTimePart.Ticks"/>
        /// </summary>
        public DateTimePart Part
        {
            get
            {
                return this.part;
            }
            set
            {
                if (this.part == value)
                {
                    return;
                }

                this.part = value;
                this.OnPropertyChanged();
            }
        }

        internal DateTime? ConvertedValue
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
        /// Prepares Where clause for current DateTime filter.
        /// </summary>
        internal override string SerializeToSQLiteWhereCondition()
        {
            long rightOperandTicks;

            switch (this.part)
            {
                case DateTimePart.Date:
                    rightOperandTicks = this.ConvertedValue.Value.Date.Ticks;
                    break;
                case DateTimePart.Time:
                    rightOperandTicks = this.ConvertedValue.Value.TimeOfDay.Ticks;
                    break;
                default:
                    rightOperandTicks = this.ConvertedValue.Value.Ticks; 
                    break;
            }

            string condition = string.Empty;
            string rightOperatndTicksString = rightOperandTicks.ToString(NumberFormatInfo.InvariantInfo);

            switch (this.filterOperator)
            {
                case NumericalOperator.EqualsTo:
                    condition = this.PropertyName + " == " + rightOperatndTicksString;
                    break;
                case NumericalOperator.DoesNotEqualTo:
                    condition = this.PropertyName + " != " + rightOperatndTicksString;
                    break;
                case NumericalOperator.IsGreaterThan:
                    condition = this.PropertyName + " > " + rightOperatndTicksString;
                    break;
                case NumericalOperator.IsGreaterThanOrEqualTo:
                    condition = this.PropertyName + " >= " + rightOperatndTicksString;
                    break;
                case NumericalOperator.IsLessThan:
                    condition = this.PropertyName + " < " + rightOperatndTicksString;
                    break;
                case NumericalOperator.IsLessThanOrEqualTo:
                    condition = this.PropertyName + " <= " + rightOperatndTicksString;
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

            DateTime dateItemValue;
            if (!TryGetDateTimeFromValue(itemValue, out dateItemValue))
            {
                return false;
            }

            long leftOperandTicks;
            long rightOperandTicks;

            switch (this.part)
            {
                case DateTimePart.Date:
                    leftOperandTicks = dateItemValue.Date.Ticks;
                    rightOperandTicks = this.convertedValue.Value.Date.Ticks;
                    break;
                case DateTimePart.Time:
                    leftOperandTicks = dateItemValue.TimeOfDay.Ticks;
                    rightOperandTicks = this.convertedValue.Value.TimeOfDay.Ticks;
                    break;
                default:
                    leftOperandTicks = dateItemValue.Ticks;
                    rightOperandTicks = this.convertedValue.Value.Ticks;
                    break;
            }

            bool passes = false;

            switch (this.filterOperator)
            {
                case NumericalOperator.EqualsTo:
                    passes = leftOperandTicks == rightOperandTicks;
                    break;
                case NumericalOperator.DoesNotEqualTo:
                    passes = leftOperandTicks != rightOperandTicks;
                    break;
                case NumericalOperator.IsGreaterThan:
                    passes = leftOperandTicks > rightOperandTicks;
                    break;
                case NumericalOperator.IsGreaterThanOrEqualTo:
                    passes = leftOperandTicks >= rightOperandTicks;
                    break;
                case NumericalOperator.IsLessThan:
                    passes = leftOperandTicks < rightOperandTicks;
                    break;
                case NumericalOperator.IsLessThanOrEqualTo:
                    passes = leftOperandTicks <= rightOperandTicks;
                    break;
            }

            return passes;
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (changedPropertyName == "Value")
            {
                this.convertedValue = null;
            }

            base.PropertyChangedOverride(changedPropertyName);
        }

        private static bool TryGetDateTimeFromValue(object value, out DateTime result)
        {
            result = DateTime.MinValue;
            if (value is DateTime)
            {
                result = (DateTime)value;
                return true;
            }

            if (value is DateTime?)
            {
                DateTime? nullable = (DateTime?)value;
                if (nullable.HasValue)
                {
                    result = nullable.Value;
                    return true;
                }
            }

            if (value is DateTimeOffset)
            {
                DateTimeOffset offset = (DateTimeOffset)value;

                // TODO: DateTime or LocalDateTime?
                result = offset.LocalDateTime;
                return true;
            }

            return false;
        }

        private void TryConvertValue()
        {
            DateTime value;
            if (this.Value is string)
            {
                if (DateTime.TryParse((string)this.Value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out value))
                {
                    this.convertedValue = value;
                }
            }
            else if (TryGetDateTimeFromValue(this.Value, out value))
            {
                this.convertedValue = value;
            }
        }
    }
}
