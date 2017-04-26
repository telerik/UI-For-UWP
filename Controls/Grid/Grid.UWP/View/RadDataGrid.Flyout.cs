using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        /// <summary>
        /// Identifies the <see cref="HideFlyoutOnBackButtonPressed"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HideFlyoutOnBackButtonPressedProperty =
            DependencyProperty.Register(nameof(HideFlyoutOnBackButtonPressed), typeof(bool), typeof(RadDataGrid), new PropertyMetadata(false, OnHideFlyoutOnBackButtonPressedChanged));

        private bool isListeningCurrentViewBackRequested = false;

        /// <summary>
        /// Gets or sets a value indicating whether information if the flyout should be hidden when the back Button gets pressed.
        /// </summary>
        public bool HideFlyoutOnBackButtonPressed
        {
            get { return (bool)GetValue(HideFlyoutOnBackButtonPressedProperty); }
            set { SetValue(HideFlyoutOnBackButtonPressedProperty, value); }
        }

        private static void OnHideFlyoutOnBackButtonPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            if (grid != null && grid.IsTemplateApplied)
            {
                if ((bool)e.NewValue && !grid.isListeningCurrentViewBackRequested)
                {
                    grid.isListeningCurrentViewBackRequested = true;
                    if (!DesignMode.DesignModeEnabled)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested += (d as RadDataGrid).RadDataGrid_BackRequested;
                    }
                }
                else
                {
                    grid.isListeningCurrentViewBackRequested = false;
                    if (!DesignMode.DesignModeEnabled)
                    {
                        SystemNavigationManager.GetForCurrentView().BackRequested -= (d as RadDataGrid).RadDataGrid_BackRequested;
                    }
                }
            }
        }

        private void RadDataGrid_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.ContentFlyout.IsOpen)
            {
                this.ContentFlyout.Hide(DataGridFlyoutId.All);
                e.Handled = true;
            }
            else if (this.ServicePanel != null && this.ServicePanel.GroupFlyout != null && this.ServicePanel.GroupFlyout.IsOpen)
            {
                this.ServicePanel.GroupFlyout.IsOpen = false;
                e.Handled = true;
            }
        }
    }
}
