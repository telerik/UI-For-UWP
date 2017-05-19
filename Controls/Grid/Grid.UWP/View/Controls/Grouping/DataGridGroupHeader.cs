using System;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Grid.View;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Allows a user to view a header and expand that header to see further details, or to collapse a section up to a header.
    /// </summary>
    [TemplateVisualState(GroupName = "ExpandedStates", Name = "Expanded")]
    [TemplateVisualState(GroupName = "ExpandedStates", Name = "Collapsed")]
    public class DataGridGroupHeader : DataGridHeader
    {
        /// <summary>
        /// Identifies the IsExpanded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(DataGridGroupHeader), new PropertyMetadata(true, OnIsExpandedPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="IndentWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IndentWidthProperty =
            DependencyProperty.Register(nameof(IndentWidth), typeof(double), typeof(DataGridGroupHeader), new PropertyMetadata(0d));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridGroupHeader"/> class.
        /// </summary>
        public DataGridGroupHeader()
        {
            this.DefaultStyleKey = typeof(DataGridGroupHeader);
        }

        internal DataGridGroupHeader(bool isFrozen)
            : this()
        {
            this.IsFrozen = isFrozen;
        }

        /// <summary>
        /// Gets a value indicating whether the details are expanded or collapsed.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(IsExpandedProperty);
            }
            internal set
            {
                this.SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width that indents the header depending on its group level.
        /// </summary>
        public double IndentWidth
        {
            get
            {
                return (double)this.GetValue(IndentWidthProperty);
            }
            set
            {
                this.SetValue(IndentWidthProperty, value);
            }
        }

        internal RadDataGrid Owner
        {
            get;
            set;
        }

        internal bool IsFrozen
        {
            get;
            private set;
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e == null)
            {
                return;
            }

            if (!e.Handled && this.Owner != null)
            {
                this.Owner.OnGroupHeaderTap(this);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called before the DoubleTapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            base.OnDoubleTapped(e);

            if (e == null)
            {
                return;
            }

            if (!e.Handled && this.Owner != null)
            {
                this.Owner.OnGroupHeaderTap(this);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.IsExpanded)
            {
                return "Expanded";
            }

            return "Collapsed";
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridGroupHeaderAutomationPeer(this);
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridGroupHeader groupHeader = d as DataGridGroupHeader;
            var context = groupHeader.DataContext as GroupHeaderContext;
            if (context != null)
            {
                context.IsExpanded = (bool)e.NewValue;
            }

            groupHeader.UpdateVisualState(groupHeader.IsTemplateApplied);
            if (groupHeader.Owner != null)
            {
                groupHeader.Owner.OnGroupIsExpandedChanged();
            }

            var peer = FrameworkElementAutomationPeer.FromElement(groupHeader) as DataGridGroupHeaderAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(!((bool)e.NewValue), (bool)e.NewValue);
            }
        }
    }
}