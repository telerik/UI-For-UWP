using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a concrete <see cref="DataGridDateTimeFilterControl"/> that allows for filtering over the <see cref="DateTime.Date"/> value.
    /// </summary>
    [TemplatePart(Name = "PART_OperatorCombo", Type = typeof(DataGridFilterComboBox))]
    [TemplatePart(Name = "PART_DatePicker", Type = typeof(RadDatePicker))]
    public class DataGridDateFilterControl : DataGridDateTimeFilterControl
    {
        private RadDatePicker datePicker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridDateFilterControl" /> class.
        /// </summary>
        public DataGridDateFilterControl()
        {
            this.DefaultStyleKey = typeof(DataGridDateFilterControl);
        }

        internal override DateTimePicker Picker
        {
            get
            {
                return this.datePicker;
            }
        }

        internal override DateTimePart Part
        {
            get
            {
                return DateTimePart.Date;
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.datePicker = this.GetTemplatePartField<RadDatePicker>("PART_DatePicker");
            applied = applied && this.datePicker != null;

            return applied;
        }
    }
}
