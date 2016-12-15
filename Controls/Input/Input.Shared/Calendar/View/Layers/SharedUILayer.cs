using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal abstract class SharedUILayer : CalendarLayer
    {
        private Panel sharedPanel;

        /// <summary>
        /// Gets the <see cref="UIElement" /> instance that is used by this layer.
        /// </summary>
        protected internal override UIElement VisualElement
        {
            get
            {
                return this.sharedPanel;
            }
        }

        /// <summary>
        /// Attaches the layer to the visual tree.
        /// </summary>
        protected internal override void AttachUI(Panel parent)
        {
            this.sharedPanel = parent;
        }

        /// <summary>
        /// Detaches the layer from the visual tree and removes any visual elements created by the layer.
        /// </summary>
        protected internal override void DetachUI(Panel parent)
        {
            this.sharedPanel = null;

            // TODO: Clear all the visuals created by the layer
        }
    }
}
