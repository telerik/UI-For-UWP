using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Contains information about the <see cref="RadDataBoundListBox.ItemTap"/> event.
    /// </summary>
    public class ListBoxItemTapEventArgs : EventArgs
    {
        private RadDataBoundListBoxItem item;
        private UIElement manipulationContainer;
        private Point manipulationOrigin;
        private UIElement originalSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxItemTapEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="manipulationContainer">The container of the item.</param>
        /// <param name="manipulationOrigin">The point at which the item was tapped.</param>
        public ListBoxItemTapEventArgs(RadDataBoundListBoxItem item, UIElement manipulationContainer, Point manipulationOrigin)
        {
            this.item = item;
            this.manipulationOrigin = manipulationOrigin;
            this.manipulationContainer = manipulationContainer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBoxItemTapEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="manipulationContainer">The container of the item.</param>
        /// <param name="originalSource">The element from which the manipulation originates.</param>
        /// <param name="manipulationOrigin">The point at which the item was tapped.</param>
        public ListBoxItemTapEventArgs(RadDataBoundListBoxItem item, UIElement manipulationContainer, UIElement originalSource, Point manipulationOrigin)
            : this(item, manipulationContainer, manipulationOrigin)
        {
            this.originalSource = originalSource;
        }

        /// <summary>
        /// Gets the element from which the manipulation originates.
        /// </summary>
        public UIElement OriginalSource
        {
            get
            {
                return this.originalSource;
            }
        }

        /// <summary>
        /// Gets the item instance that was clicked.
        /// </summary>
        public RadDataBoundListBoxItem Item
        {
            get
            {
                return this.item;
            }
        }

        /// <summary>
        /// Gets the point where the Tap gesture originated.
        /// </summary>
        public Point ManipulationOrigin
        {
            get
            {
                return this.manipulationOrigin;
            }
        }

        /// <summary>
        /// Gets the <see cref="UIElement"/> descendant of the associated item where the manipulation originated.
        /// </summary>
        public UIElement ManipulationContainer
        {
            get
            {
                return this.manipulationContainer;
            }
        }
    }
}
