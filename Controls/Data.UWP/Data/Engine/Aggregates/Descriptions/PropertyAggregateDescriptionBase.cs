using System;
using Telerik.Data.Core.Aggregates;
using Telerik.Data.Core.Fields;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Base class that describes the aggregation of items using a property name as the criteria.
    /// </summary>
    internal abstract class PropertyAggregateDescriptionBase : AggregateDescriptionBase
    {
        private string stringFormat;
        private AggregateFunction aggregateFunction;
        private StringFormatSelector stringFormatSelector;

        /// <summary>
        /// Gets or sets the aggregate function that will be used for summary calculation.
        /// </summary>
        public AggregateFunction AggregateFunction
        {
            get
            {
                if (this.aggregateFunction == null)
                {
                    this.aggregateFunction = new SumAggregateFunction();
                }

                return this.aggregateFunction;
            }

            set
            {
                if (this.aggregateFunction != value)
                {
                    this.ChangeSettingsProperty(ref this.aggregateFunction, value);
                    this.OnPropertyChanged(nameof(AggregateFunction));
                    this.OnPropertyChanged(nameof(this.DisplayName));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets a general string format to use for this <see cref="AggregateDescriptionBase"/> <see cref="AggregateValue"/>.
        /// This format will be used if the <see cref="AggregateFunction"/> or <see cref="Telerik.Data.Core.Totals.TotalFormat"/> does not alter the meaning of the original data.
        /// </summary>
        public string StringFormat
        {
            get
            {
                return this.stringFormat;
            }

            set
            {
                if (this.stringFormat != value)
                {
                    this.stringFormat = value;
                    this.OnPropertyChanged(nameof(this.StringFormat));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="StringFormatSelector"/> that would provide a proper StringFormat for
        /// <see cref="AggregateFunction"/> or <see cref="Telerik.Data.Core.Totals.TotalFormat"/> that alter the meaning of the original data.
        /// <see cref="StringFormat"/>.
        /// <see cref="Telerik.Data.Core.Totals.TotalFormat"/>.
        /// </summary>
        internal StringFormatSelector StringFormatSelector
        {
            get
            {
                return this.stringFormatSelector;
            }

            set
            {
                if (this.stringFormatSelector != value)
                {
                    this.stringFormatSelector = value;
                    this.OnPropertyChanged(nameof(StringFormatSelector));
                }
            }
        }

        /// <summary>
        /// Gets a string format suitable to format the <see cref="AggregateValue"/> created by this <see cref="PropertyAggregateDescriptionBase"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Design choice.")]
        public string GetEffectiveFormat()
        {
            var fieldDescription = this.MemberAccess as IDataFieldInfo;
            var dataType = fieldDescription == null ? null : fieldDescription.DataType;
            return Telerik.Data.Core.Aggregates.StringFormatSelector.SelectStringFormat(this.StringFormatSelector, dataType, this);
        }

        /// <summary>
        /// Returns AggregateValue instance based on the AggregateFunction value.
        /// </summary>
        /// <returns>Returns AggregateValue used for summary calculation.</returns>
        protected internal sealed override AggregateValue CreateAggregate()
        {
            // NOTE: To extend you must extend the PropertyAggregateDescriptionBase,
            // We may implement precision as a double dispatch here based on the runtime type of the item and the this.AggregateFunction
            var fieldDescription = this.MemberAccess as IDataFieldInfo;
            var dataType = fieldDescription == null ? null : fieldDescription.DataType;
            return this.AggregateFunction.CreateAggregate(dataType);
        }

        /// <summary>
        /// Returns the value that will be passed in the aggregate for given item.
        /// </summary>
        /// <param name="item">The item which value will be extracted.</param>
        /// <returns>Returns the value for given item.</returns>
        protected internal virtual object GetValueForItem(object item)
        {
            if (this.MemberAccess == null)
            {
                throw new InvalidOperationException("Member access has not been initialized");
            }

            var value = this.MemberAccess.GetValue(item);
            var processedValue = ReturnInvalidValuesAsNull(value);

            return processedValue;
        }

        /// <inheritdoc />
        protected override string GetDisplayName()
        {
            if (string.IsNullOrEmpty(this.CustomName))
            {
                return this.AggregateFunction.ToString() + " of " + this.PropertyName;
            }

            return this.CustomName;
        }

        /// <inheritdoc />
        protected override sealed void CloneCore(Cloneable source)
        {
            this.CloneOverride(source);
            PropertyAggregateDescriptionBase original = source as PropertyAggregateDescriptionBase;
            if (original != null)
            {
                this.AggregateFunction = Cloneable.CloneOrDefault(original.AggregateFunction);
                this.StringFormatSelector = Cloneable.CloneOrDefault(original.StringFormatSelector);
                this.StringFormat = original.StringFormat;
            }

            base.CloneCore(source);
        }

        /// <summary>
        /// Makes the instance a clone (deep copy) of the specified <see cref="Telerik.Data.Core.Cloneable"/>.
        /// </summary>
        /// <param name="source">The object to clone.</param>
        /// <remarks>Notes to Inheritors
        /// If you derive from <see cref="Telerik.Data.Core.Cloneable"/>, you need to override this method to copy all properties.
        /// It is essential that all implementations call the base implementation of this method (if you don't call base you should manually copy all needed properties including base properties).
        /// </remarks>
        protected abstract void CloneOverride(Cloneable source);

        private static object ReturnInvalidValuesAsNull(object value)
        {
            // TODO: Preparation for DBNULL (remove the private DBNull class when WinRT supports DBNull.
            if (value == DBNull.Value)
            {
                return null;
            }

            return value;
        }

        private sealed class DBNull
        {
            public static readonly DBNull Value = new DBNull();

            private DBNull()
            {
            }
        }
    }
}