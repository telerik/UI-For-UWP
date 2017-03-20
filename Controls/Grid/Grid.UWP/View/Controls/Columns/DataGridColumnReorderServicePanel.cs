using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the service UI that controls the column reorder flyout.
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "CommonStates")]
    public class DataGridColumnReorderServicePanel : RadControl
    {
        private DataGridColumnsFlyout columnReorderFlyoutContent = new DataGridColumnsFlyout();
        private RadDataGrid owner;
        private bool isMouseOver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnReorderServicePanel" /> class.
        /// </summary>
        public DataGridColumnReorderServicePanel()
        {
            this.DefaultStyleKey = typeof(DataGridColumnReorderServicePanel);
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

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.Owner.ContentFlyout.Opened += this.ColumnReorderFlyout_Opened;
            this.Owner.ContentFlyout.Closed += this.ColumnReorderFlyout_Closed;

            this.columnReorderFlyoutContent.Owner = this;
        }

        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.Owner.ContentFlyout.Opened -= this.ColumnReorderFlyout_Opened;
            this.Owner.ContentFlyout.Closed -= this.ColumnReorderFlyout_Closed;

            this.columnReorderFlyoutContent.Owner = null;
        }
        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e == null || this.Owner == null)
            {
                return;
            }

            this.OpenColumnsFlyout();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            this.isMouseOver = true;
            this.UpdateVisualState(false);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            this.isMouseOver = false;
            this.UpdateVisualState(false);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridColumnReorderServicePanelAutomationPeer(this);
        }

        internal void OpenColumnsFlyout()
        {
            this.columnReorderFlyoutContent.ClearUI();

            this.columnReorderFlyoutContent.PrepareUI();
            this.PositionColumnsFlyout();
            this.columnReorderFlyoutContent.UpdateVisualState(false);
            this.Owner.ContentFlyout.Show(DataGridFlyoutId.ColumnChooser, this.columnReorderFlyoutContent);
        }

        internal void CloseColumnsFlyout()
        {
            this.Owner.ContentFlyout.Hide(DataGridFlyoutId.ColumnChooser);
        }

        protected override string ComposeVisualStateName()
        {
            if (this.isMouseOver)
            {
                return "NormalPointerOver";
            }
            else if (this.Owner.ContentFlyout.IsOpen)
            {
                return "Expanded";
            }
            else
            {
                return "Normal";
            }
        }

        private void PositionColumnsFlyout()
        {
            if (this.owner.ActualHeight < this.owner.ActualWidth)
            {
                this.columnReorderFlyoutContent.Width = double.NaN;
                this.columnReorderFlyoutContent.Height = this.owner.ActualHeight;
            }
            else
            {
                this.columnReorderFlyoutContent.Width = this.owner.ActualWidth;
                this.columnReorderFlyoutContent.Height = double.NaN;
            }
        }

        private void ColumnReorderFlyout_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.owner.ContentFlyout.Id != DataGridFlyoutId.ColumnChooser)
            {
                return;
            }

            this.PositionColumnsFlyout();
        }

        private void ColumnReorderFlyout_Closed(object sender, object e)
        {
            if (this.owner.ContentFlyout.Id != DataGridFlyoutId.ColumnChooser)
            {
                return;
            }

            this.UpdateVisualState(this.IsTemplateApplied);
        }

        private void ColumnReorderFlyout_Opened(object sender, object e)
        {
            this.UpdateVisualState(this.IsTemplateApplied);
        }
    }
}
