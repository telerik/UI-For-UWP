using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents Indicator controlled used by DataGrid when auto loading more data items.
    /// </summary>
    public class DataGridAutoDataLoadingControl : RadControl, IDataStatusListener
    {
        /// <summary>
        /// Identifies the <see cref="LoadingData"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LoadingDataProperty =
            DependencyProperty.Register(nameof(LoadingData), typeof(bool), typeof(DataGridAutoDataLoadingControl), new PropertyMetadata(false, OnLoadingDataChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridAutoDataLoadingControl" /> class.
        /// </summary>
        public DataGridAutoDataLoadingControl()
        {
            this.DefaultStyleKey = typeof(DataGridAutoDataLoadingControl);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is loading data state.
        /// </summary>
        public bool LoadingData
        {
            get { return (bool)this.GetValue(LoadingDataProperty); }
            set { this.SetValue(LoadingDataProperty, value); }
        }

        void IDataStatusListener.OnDataStatusChanged(Telerik.Data.Core.DataProviderStatus status)
        {
            this.LoadingData = status == DataProviderStatus.RequestingData;
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            return this.LoadingData ? "Loading" : base.ComposeVisualStateName();
        }

        private static void OnLoadingDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGridAutoDataLoadingControl;

            control.UpdateVisualState(true);
        }
    }
}
