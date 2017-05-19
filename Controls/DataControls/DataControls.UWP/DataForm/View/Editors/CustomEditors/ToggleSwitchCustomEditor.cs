using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a ToggleSwitchCustomEditor control.
    /// </summary>
    public class ToggleSwitchCustomEditor : CustomEditorBase<ToggleSwitch>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitchCustomEditor"/> class.
        /// </summary>
        public ToggleSwitchCustomEditor()
        {
            this.DefaultStyleKey = typeof(ToggleSwitchCustomEditor);
        }
    }
}
