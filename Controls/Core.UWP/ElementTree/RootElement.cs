using System;

namespace Telerik.Core
{
    /// <summary>
    /// Represents the root element of a logical tree. This element is usually aggregated by a <see cref="IView"/> instance.
    /// </summary>
    public abstract class RootElement : Element
    {
        /// <summary>
        /// Determines whether the node is loaded.
        /// </summary>
        public override bool IsTreeLoaded
        {
            get
            {
                if (this.View == null)
                {
                    return false;
                }

                return this.nodeState == NodeState.Loaded;
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="IView"/> instance that aggregates this element.
        /// </summary>
        protected internal IView View
        {
            get;
            set;
        }

        /// <summary>
        /// Arranges the element using a rectangle located at (0, 0) and with Size equal to the current view's Viewport.
        /// </summary>
        public void Arrange()
        {
            if (!this.IsTreeLoaded)
            {
                return;
            }

            double width = this.View.ViewportWidth;
            double height = this.View.ViewportHeight;

            this.Arrange(new RadRect(0, 0, width, height));
        }

        internal void InvalidateNode(Node node)
        {
            if (!this.IsTreeLoaded)
            {
                return;
            }

            IElementPresenter presenter = node.Presenter;
            if (presenter != null)
            {
                presenter.RefreshNode(node);
            }
            else
            {
                this.View.RefreshNode(node);
            }
        }
    }
}
