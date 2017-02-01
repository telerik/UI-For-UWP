using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal abstract class CalendarLayer : AttachableObject<RadCalendar>
    {
        /// <summary>
        /// Gets the <see cref="UIElement"/> instance that is used by this layer.
        /// </summary>
        protected internal abstract UIElement VisualElement
        {
            get;
        }

        internal static void ArrangeUIElement(FrameworkElement presenter, RadRect layoutSlot, bool setSize = true)
        {
            if (presenter == null)
            {
                return;
            }

            Canvas.SetLeft(presenter, layoutSlot.X);
            Canvas.SetTop(presenter, layoutSlot.Y);

            if (setSize)
            {
                presenter.Width = layoutSlot.Width;
                presenter.Height = layoutSlot.Height;
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
