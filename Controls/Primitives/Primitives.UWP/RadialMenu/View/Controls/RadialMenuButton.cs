using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Represents the button control used to open the <see cref="RadRadialMenu"/> component. It is also used as a back button when navigating through the item childs.
    /// </summary>
    public class RadialMenuButton : Button
    {
        private bool displayBackContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialMenuButton" /> class.
        /// </summary>
        public RadialMenuButton()
        {
            this.DefaultStyleKey = typeof(RadialMenuButton);
            Canvas.SetZIndex(this, ZIndices.MainButtonZIndex);
        }

        internal bool DisplayBackContent
        {
            get
            {
                return this.displayBackContent;
            }
            set
            {
                this.displayBackContent = value;
            }
        }

        internal void TransformToBackButton()
        {
            this.DisplayBackContent = true;
            VisualStateManager.GoToState(this, "Back", true);
        }

        internal void TransformToNormal()
        {
            this.DisplayBackContent = false;
            VisualStateManager.GoToState(this, "Home", true);
        }
    }
}
