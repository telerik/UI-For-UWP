namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the custom Control implementation used to visualize the current item within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public class DataGridCurrencyControl : RadControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridCurrencyControl" /> class.
        /// </summary>
        public DataGridCurrencyControl()
        {
            this.DefaultStyleKey = typeof(DataGridCurrencyControl);
            
            this.IsHitTestVisible = false;
        }
    }
}
