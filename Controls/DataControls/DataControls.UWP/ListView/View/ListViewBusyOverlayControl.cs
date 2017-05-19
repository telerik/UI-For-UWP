using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents busy indicator overlay used by <see cref="RadListView"/> when background operation is in progress.
    /// </summary>
    public class ListViewBusyOverlayControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="IsBusy"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(ListViewBusyOverlayControl), new PropertyMetadata(false, OnIsBusyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewBusyOverlayControl"/> class.
        /// </summary>
        public ListViewBusyOverlayControl()
        {
            this.DefaultStyleKey = typeof(ListViewBusyOverlayControl);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is in busy state.
        /// </summary>
        /// <value>The is busy.</value>
        public bool IsBusy
        {
            get { return (bool)this.GetValue(IsBusyProperty); }
            set { this.SetValue(IsBusyProperty, value); }
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            return this.IsBusy ? "Busy" : base.ComposeVisualStateName();
        }

        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewBusyOverlayControl;

            control.UpdateVisualState(true);
        }
    }
}
