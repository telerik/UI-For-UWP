using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a SliderCustomEditor control.
    /// </summary>
    public class SliderCustomEditor : CustomEditorBase<Slider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderCustomEditor"/> class.
        /// </summary>
        public SliderCustomEditor()
        {
            this.DefaultStyleKey = typeof(SliderCustomEditor);
        }
    }
}
