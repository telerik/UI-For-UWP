using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a panel that contains the frozen group rows.
    /// </summary>
    public class FrozenGroupsPanel : Panel
    {
        private RectangleGeometry clipGeometry = new RectangleGeometry();

        /// <summary>
        /// Initializes a new instance of the FrozenGroupsPanel class.  
        /// </summary>
        public FrozenGroupsPanel()
        {
            this.Clip = this.clipGeometry;
            this.SizeChanged += this.OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.clipGeometry.Rect = new Rect(new Point(), e.NewSize);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrozenGroupsPanelAutomationPeer(this);
        }
    }
}