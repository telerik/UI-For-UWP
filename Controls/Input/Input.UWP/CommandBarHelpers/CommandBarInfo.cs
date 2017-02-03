using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.DateTimePickers
{
    /// <summary>
    /// This class holds information about the command bar of the <see cref="DateTimePicker"/>.
    /// </summary>
    public class CommandBarInfo : Control
    {
        /// <summary>
        /// Identifies the <see cref="PickerCommandBar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PickerCommandBarProperty =
            DependencyProperty.Register(nameof(PickerCommandBar), typeof(CommandBar), typeof(CommandBarInfo), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the <see cref="CommandBar"/> used by the <see cref="DateTimePicker"/> control.
        /// </summary>
        public CommandBar PickerCommandBar
        {
            get
            {
                return (CommandBar)this.GetValue(PickerCommandBarProperty);
            }
            set
            {
                this.SetValue(PickerCommandBarProperty, value);
            }
        }
    }
}
