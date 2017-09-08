using System;
using System.Collections.Generic;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the base class used to filter <see cref="DateTime"/> data.
    /// </summary>
    public abstract class DataGridDateTimeFilterControl : DataGridTypedFilterControl
    {
        internal abstract DateTimePicker Picker
        {
            get;
        }

        internal abstract DateTimePart Part
        {
            get;
        }

        /// <summary>
        /// Builds the <see cref="FilterDescriptorBase" /> that describes the user input within this instance.
        /// </summary>
        public override FilterDescriptorBase BuildDescriptor()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.PropertyName = this.PropertyName;
            descriptor.Operator = (NumericalOperator)this.OperatorCombo.SelectedIndex;

            if (this.Picker != null)
            {
                descriptor.Value = this.Picker.Value;
                descriptor.Part = this.Part;
            }

            return descriptor;
        }

        /// <summary>
        /// Initializes the control depending on the current <see cref="P:AssociatedDescriptor" /> value.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var dateTimeDescriptor = this.ActualAssociatedDescriptor as DateTimeFilterDescriptor;
            string selectedItem;

            if (dateTimeDescriptor != null)
            {
                selectedItem = GridLocalizationManager.Instance.GetString(dateTimeDescriptor.Operator.ToString());
                if (this.Picker != null)
                {
                    this.Picker.Value = dateTimeDescriptor.ConvertedValue;
                }
            }
            else
            {
                selectedItem = GridLocalizationManager.Instance.GetString(NumericalOperator.EqualsTo.ToString());
            }

            this.OperatorCombo.SelectedItem = selectedItem;
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetOperators()
        {
            foreach (var op in Enum.GetValues(typeof(NumericalOperator)))
            {
                yield return GridLocalizationManager.Instance.GetString(op.ToString());
            }
        }
    }
}
