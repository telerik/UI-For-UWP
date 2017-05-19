using System;
using System.Collections.Generic;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a concrete <see cref="DataGridTypedFilterControl"/> that allows for filtering of Numerical data.
    /// </summary>
    [TemplatePart(Name = "PART_OperatorCombo", Type = typeof(DataGridFilterComboBox))]
    [TemplatePart(Name = "PART_ValueBox", Type = typeof(RadNumericBox))]
    public class DataGridNumericalFilterControl : DataGridTypedFilterControl
    {
        private RadNumericBox valueBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridNumericalFilterControl" /> class.
        /// </summary>
        public DataGridNumericalFilterControl()
        {
            this.DefaultStyleKey = typeof(DataGridNumericalFilterControl);
        }

        /// <summary>
        /// Gets the numeric box. Exposed for testing purposes only.
        /// </summary>
        internal RadNumericBox NumericBox
        {
            get
            {
                return this.valueBox;
            }
        }

        /// <summary>
        /// Builds the <see cref="FilterDescriptorBase" /> that describes the user input within this instance.
        /// </summary>
        public override FilterDescriptorBase BuildDescriptor()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.PropertyName = this.PropertyName;
            descriptor.Operator = (NumericalOperator)this.OperatorCombo.SelectedIndex;
            descriptor.Value = this.valueBox.Value;

            return descriptor;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.valueBox = this.GetTemplatePartField<RadNumericBox>("PART_ValueBox");
            applied = applied && this.valueBox != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.valueBox.Minimum = double.MinValue;
            this.valueBox.Maximum = double.MaxValue;
        }

        /// <summary>
        /// Initializes the control depending on the current <see cref="P:AssociatedDescriptor" /> value.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var numericDescriptor = this.ActualAssociatedDescriptor as NumericalFilterDescriptor;
            string selectedItem;

            if (numericDescriptor != null)
            {
                selectedItem = GridLocalizationManager.Instance.GetString(numericDescriptor.Operator.ToString());

                double? value = numericDescriptor.ConvertedValue;
                if (value.HasValue)
                {
                    this.valueBox.Value = value;
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
