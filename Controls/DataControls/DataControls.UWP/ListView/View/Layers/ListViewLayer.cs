using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal abstract class ListViewLayer : AttachableObject<RadListView>
    {
        private Panel panel;

        /// <summary>
        /// Gets the <see cref="UIElement"/> instance that is used by this layer.
        /// </summary>
        protected internal virtual UIElement VisualElement
        {
            get
            {
                return this.panel;
            }
        }

        /// <summary>
        /// Attaches the layer to the visual tree.
        /// </summary>
        protected internal virtual void AttachUI(Panel parent)
        {
            this.panel = parent;
        }

        /// <summary>
        /// Detaches the layer from the visual tree and removes any visual elements created by the layer.
        /// </summary>
        protected internal virtual void DetachUI(Panel parent)
        {
            this.panel = null;
        }

        /// <summary>
        /// Adds the specified <see cref="UIElement"/> to the visual container associated with the layer.
        /// </summary>
        protected internal virtual void AddVisualChild(UIElement child)
        {
            Panel panel = this.VisualElement as Panel;
            if (panel != null)
            {
                panel.Children.Add(child);
            }
        }

        /// <summary>
        /// Removes the specified <see cref="UIElement"/> from the visual container associated with the layer.
        /// </summary>
        protected internal virtual void RemoveVisualChild(UIElement child)
        {
            Panel panel = this.VisualElement as Panel;
            if (panel != null)
            {
                panel.Children.Remove(child);
            }
        }
    }
}
