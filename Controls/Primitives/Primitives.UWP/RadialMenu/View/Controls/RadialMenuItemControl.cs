using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    /// <summary>
    /// Represents the custom Control implementation used to visualize a <see cref="RadialMenuItem"/> and its children.
    /// </summary>
    public class RadialMenuItemControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(RadialMenuItemControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IconContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconContentProperty =
            DependencyProperty.Register(nameof(IconContent), typeof(object), typeof(RadialMenuItemControl), new PropertyMetadata(null));

        internal RadialSegment Segment;

        /// <summary>
        /// Identifies the <see cref="Loading"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty LoadingProperty =
            DependencyProperty.Register(nameof(Loading), typeof(bool), typeof(RadialMenuItemControl), new PropertyMetadata(true, OnLoadingChanged));

        private bool isPointerOver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialMenuItemControl" /> class.
        /// </summary>
        public RadialMenuItemControl()
        {
            this.DefaultStyleKey = typeof(RadialMenuItemControl);
        }

        /// <summary>
        /// Gets or sets the visual icon content of the current <see cref="RadialMenuItem"/>.
        /// </summary>
        public object IconContent
        {
            get
            {
                return (object)GetValue(IconContentProperty);
            }
            set
            {
                this.SetValue(IconContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visual header of the current <see cref="RadialMenuItem"/>.
        /// </summary>
        public object Header
        {
            get
            {
                return (object)GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        internal new bool Loading
        {
            get
            {
                return (bool)this.GetValue(LoadingProperty);
            }
            set
            {
                this.SetValue(LoadingProperty, value);
            }
        }

        internal bool IsPointerOver
        {
            get
            {
                return this.isPointerOver;
            }
            set
            {
                this.isPointerOver = value;
                this.UpdateVisualState(true);
            }
        }

        internal void Update()
        {
            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            string commonVisualState = base.ComposeVisualStateName();
            string selectionVisualState = (this.Segment != null && this.Segment.TargetItem.IsSelected) ? "Selected" : commonVisualState;
            commonVisualState = (this.Segment != null && this.IsPointerOver) ? "PointerOver" : selectionVisualState;
            commonVisualState = this.IsEnabled ? commonVisualState : base.ComposeVisualStateName();

            commonVisualState = this.Loading ? "Loading" : commonVisualState;

            return commonVisualState;
        }

        private static void OnLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadialMenuItemControl;

            control.UpdateVisualState(true);
        }
    }
}
