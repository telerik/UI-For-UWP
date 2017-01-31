using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a concrete <see cref="DataGridDateTimeFilterControl"/> that allows for filtering over the <see cref="DateTime.TimeOfDay"/> value.
    /// </summary>
    [TemplatePart(Name = "PART_OperatorCombo", Type = typeof(DataGridFilterComboBox))]
    [TemplatePart(Name = "PART_TimePicker", Type = typeof(RadTimePicker))]
    public class DataGridTimeFilterControl : DataGridDateTimeFilterControl
    {
        private RadTimePicker timePicker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridTimeFilterControl" /> class.
        /// </summary>
        public DataGridTimeFilterControl()
        {
            this.DefaultStyleKey = typeof(DataGridTimeFilterControl);
        }

        internal override DateTimePicker Picker
        {
            get
            {
                return this.timePicker;
            }
        }

        internal override DateTimePart Part
        {
            get
            {
                return DateTimePart.Time;
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.timePicker = this.GetTemplatePartField<RadTimePicker>("PART_TimePicker");
            applied = applied && this.timePicker != null;

            return applied;
        }
    }
}
