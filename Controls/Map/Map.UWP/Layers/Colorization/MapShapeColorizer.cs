using System;
using System.Collections.Generic;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a class that may be used to colorize a <see cref="IMapShape"/> instance, present within a <see cref="MapShapeLayer"/>.
    /// </summary>
    public abstract class MapShapeColorizer : RadDependencyObject
    {
        private bool isInitialized;

        /// <summary>
        /// Raised when the current instance has changed in a way that needs re-evaluation of each shape's style.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Gets a value indicating whether the colorizer is successfully initialized and has its colorization logic realized.
        /// </summary>
        public bool IsProperlyInitialized
        {
            get
            {
                return this.isInitialized;
            }
        }

        internal void Reset()
        {
            this.ResetOverride();
            this.isInitialized = false;
        }

        internal void Initialize(IEnumerable<IMapShape> shapes)
        {
            this.Reset();
            if (shapes == null)
            {
                return;
            }

            this.isInitialized = this.InitializeOverride(shapes);

            if (this.isInitialized)
            {
                this.OnInitialized();
            }
        }

        /// <summary>
        /// Gets the <see cref="D2DShapeStyle"/> instance that defines the appearance of the specified <see cref="IMapShape"/> instance.
        /// </summary>
        /// <param name="shape">The <see cref="IMapShape"/> instance for which the style is to be retrieved.</param>
        protected internal abstract D2DShapeStyle GetShapeStyle(IMapShape shape);

        /// <summary>
        /// Provides an extension methods to inheritors to reset the current state.
        /// </summary>
        protected abstract void ResetOverride();

        /// <summary>
        /// Provides an extension methods to inheritors to perform the core initialization logic given a set of shapes.
        /// </summary>
        protected abstract bool InitializeOverride(IEnumerable<IMapShape> shapes);

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        protected virtual void OnChanged()
        {
            var eh = this.Changed;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Provides an entry point that allows inheritors to perform some additional logic upon initialization completion.
        /// </summary>
        protected virtual void OnInitialized()
        {
        }
    }
}
