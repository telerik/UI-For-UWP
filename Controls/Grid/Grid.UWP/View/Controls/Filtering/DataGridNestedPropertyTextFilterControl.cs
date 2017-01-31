using System;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a concrete <see cref="DataGridTypedFilterControl"/> that is used to filter data presented by a <see cref="DataGridTextColumn"/> instance.
    /// </summary> 
    [TemplatePart(Name = "PART_OperatorCombo", Type = typeof(DataGridFilterComboBox))]
    [TemplatePart(Name = "PART_ValueBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_CaseButton", Type = typeof(ToggleButton))]
    public class DataGridNestedPropertyTextFilterControl : DataGridTextFilterControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridNestedPropertyTextFilterControl" /> class.
        /// </summary>
        public DataGridNestedPropertyTextFilterControl()
        {
            this.DefaultStyleKey = typeof(DataGridNestedPropertyTextFilterControl);
        }

        /// <summary>
        /// Gets or sets a custom function that will extract the property from the data item to be used by the <see cref="DataGridNestedPropertyTextFilterControl"/> instance.
        /// </summary>
        public Func<object, object> ItemPropertyGetter { get; set; }

        /// <inheritdoc/>
        public override FilterDescriptorBase BuildDescriptor()
        {
            var descriptor = new DataGridNestedPropertyTextFilterDescriptor();

            descriptor.Operator = (TextOperator)this.OperatorCombo.SelectedIndex;
            descriptor.Value = this.TextBox.Text;
            descriptor.IsCaseSensitive = this.IsCaseSensitive;
            descriptor.ItemPropertyGetter = this.ItemPropertyGetter;
            descriptor.PropertyName = this.PropertyName;

            return descriptor;
        }
    }
}