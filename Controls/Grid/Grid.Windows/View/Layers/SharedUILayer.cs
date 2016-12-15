using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Represents a layer that shares common UI - like Canvas or other Panel with other layers. For example such layers are <see cref="DecorationLayer"/> and <see cref="SelectionLayer"/>.
    /// </summary>
    internal abstract class SharedUILayer : DataGridLayer
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
