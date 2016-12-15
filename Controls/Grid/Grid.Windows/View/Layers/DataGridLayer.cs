using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Represents a logical layer within a <see cref="RadDataGrid"/> control, which handles specific visualization part of the component's appearance.
    /// </summary>
    internal abstract class DataGridLayer : AttachableObject<RadDataGrid>
    {
        /// <summary>
        /// Gets the <see cref="UIElement"/> instance that is used by this layer.
        /// </summary>
        protected internal abstract UIElement VisualElement
        {
            get;
        }

        internal void RegisterUpdate(UpdateFlags flags)
        {
            if (this.Owner != null)
            {
                this.Owner.updateService.RegisterUpdate((int)flags);
            }
        }

        internal virtual void ApplyClip(RadRect clip, bool offsetWithGridLines = true)
        {
            if (this.VisualElement != null)
            {
                if (clip.IsSizeValid())
                {
                    var offset = offsetWithGridLines ? this.Owner.GridLinesThickness : 0;
                    var trimmedClip = new Windows.Foundation.Rect(clip.X + offset, clip.Y, clip.Width, clip.Height);

                    this.VisualElement.Clip = new Windows.UI.Xaml.Media.RectangleGeometry() { Rect = trimmedClip };
                }
                else
                {
                    this.VisualElement.Clip = null;
                }
            }
        }

        /// <summary>
        /// Attaches the layer to the visual tree.
        /// </summary>
        protected internal virtual void AttachUI(Panel parent)
        {
            parent.Children.Add(this.VisualElement);
        }

        /// <summary>
        /// Detaches the layer from the visual tree and removes any visual elements created by the layer.
        /// </summary>
        protected internal virtual void DetachUI(Panel parent)
        {
            parent.Children.Remove(this.VisualElement);
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
