using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents loading control used to request more data items in DataGrid.
    /// </summary>
    public class DataGridLoadDataControl : RadControl, IDataStatusListener
    {
        /// <summary>
        /// Identifies the <see cref="LoadingData"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LoadingDataProperty =
            DependencyProperty.Register(nameof(LoadingData), typeof(bool), typeof(DataGridLoadDataControl), new PropertyMetadata(false, OnLoadingDataChanged));
        private Button loadDataButton;

        private RadDataGrid owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridLoadDataControl" /> class.
        /// </summary>
        public DataGridLoadDataControl()
        {
            this.DefaultStyleKey = typeof(DataGridLoadDataControl);
        }

        internal event EventHandler LoadDataButtonClick;

        /// <summary>
        /// Gets or sets a value indicating whether the control is loading data state.
        /// </summary>
        public bool LoadingData
        {
            get { return (bool)this.GetValue(LoadingDataProperty); }
            set { this.SetValue(LoadingDataProperty, value); }
        }

        /// <summary>
        /// Gets the Text used for visualization when the more rows are loaded.
        /// </summary>
        public string LoadMoreRowsText
        {
            get
            {
                return GridLocalizationManager.Instance.GetString("LoadMoreRows");
            }
        }

        internal RadDataGrid Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        void IDataStatusListener.OnDataStatusChanged(DataProviderStatus status)
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

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.loadDataButton != null)
            {
                this.loadDataButton.Click -= this.LoadDataButton_Click;
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate"/> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            this.loadDataButton = this.GetTemplatePartField<Button>("PART_LoadButton");

            return base.ApplyTemplateCore() && this.loadDataButton != null;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.loadDataButton.Click += this.LoadDataButton_Click;
        }

        private static void OnLoadingDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DataGridLoadDataControl;

            control.UpdateVisualState(true);
        }

        private void LoadDataButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Owner.CommandService.ExecuteCommand(Commands.CommandId.LoadMoreData, new LoadMoreDataContext());

            this.RaiseLoadDataButtonClick();
        }

        private void RaiseLoadDataButtonClick()
        {
            var eh = this.LoadDataButtonClick;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }
    }
}
