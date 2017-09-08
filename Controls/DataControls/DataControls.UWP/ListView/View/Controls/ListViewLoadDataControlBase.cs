using Telerik.Data.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// A basic class for all controls used by the <see cref="RadListView"/> for incremental data loading.
    /// </summary>
    public abstract class ListViewLoadDataControlBase : RadControl, IDataStatusListener
    {
        /// <summary>
        /// Identifies the <see cref="IsLoadingData"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingDataProperty =
            DependencyProperty.Register(nameof(IsLoadingData), typeof(bool), typeof(ListViewLoadDataControlBase), new PropertyMetadata(false, OnIsLoadingDataChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the control is loading data state.
        /// </summary>
        /// <value>The loading data.</value>
        public bool IsLoadingData
        {
            get
            {
                return (bool)this.GetValue(IsLoadingDataProperty);
            }
            set
            {
                this.SetValue(IsLoadingDataProperty, value);
            }
        }

        void IDataStatusListener.OnDataStatusChanged(DataProviderStatus status)
        {
            this.IsLoadingData = status == DataProviderStatus.RequestingData;
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            return this.IsLoadingData ? "Loading" : base.ComposeVisualStateName();
        }

        private static void OnIsLoadingDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewLoadDataControlBase;
            control.UpdateVisualState(true);
        }
    }
}
