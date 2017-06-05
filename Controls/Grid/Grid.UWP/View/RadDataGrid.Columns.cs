using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        /// <summary>
        /// Identifies the <see cref="FrozenColumnCount"/> dependency property. 
        /// </summary> 
        public static readonly DependencyProperty FrozenColumnCountProperty =
            DependencyProperty.Register(nameof(FrozenColumnCount), typeof(int), typeof(RadDataGrid), new PropertyMetadata(0, OnFrozenColumnCountChanged));

        /// <summary>
        /// Identifies the <see cref="AutoGenerateColumns"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty AutoGenerateColumnsProperty =
            DependencyProperty.Register(nameof(AutoGenerateColumns), typeof(bool), typeof(RadDataGrid), new PropertyMetadata(true, OnAutoGenerateColumnsChanged));

        /// <summary>
        /// Identifies the <see cref="ColumnDataOperationsMode"/> dependency property. 
        /// </summary> 
        public static readonly DependencyProperty ColumnDataOperationsModeProperty =
            DependencyProperty.Register(nameof(ColumnDataOperationsMode), typeof(ColumnDataOperationsMode), typeof(RadDataGrid), new PropertyMetadata(ColumnDataOperationsMode.Inline, OnColumnDataOperationsModeChanged));

        /// <summary>
        /// Identifies the <see cref="CanUserChooseColumns"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CanUserChooseColumnsProperty =
            DependencyProperty.Register(nameof(CanUserChooseColumns), typeof(bool), typeof(RadDataGrid), new PropertyMetadata(false, OnCanUserChooseColumnsChanged));
        
        /// <summary>
        /// Gets the collection of <see cref="DataGridColumn"/> objects currently displayed within the grid.
        /// </summary>
        public DataGridColumnCollection Columns
        {
            get
            {
                return this.model.columns;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grid's columns will be auto-generated from the provided ViewModel's properties.
        /// </summary>
        public bool AutoGenerateColumns
        {
            get
            {
                return this.model.AutoGenerateColumns;
            }
            set
            {
                this.SetValue(AutoGenerateColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the  count of the frozen columns.
        /// </summary>
        public int FrozenColumnCount
        {
            get
            {
                return (int)this.GetValue(FrozenColumnCountProperty);
            }
            set
            {
                this.SetValue(FrozenColumnCountProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Data Operations Mode of the ColumnHeader.
        /// </summary>
        public ColumnDataOperationsMode ColumnDataOperationsMode
        {
            get
            {
                return (ColumnDataOperationsMode)GetValue(ColumnDataOperationsModeProperty);
            }
            set
            {
                SetValue(ColumnDataOperationsModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to choose which columns are visible.
        /// </summary>
        public bool CanUserChooseColumns
        {
            get
            {
                return (bool)this.GetValue(CanUserChooseColumnsProperty);
            }
            set
            {
                this.SetValue(CanUserChooseColumnsProperty, value);
            }
        }

        internal void OnColumnHeaderSelectionChanged(DataGridColumnHeader dataGridColumnHeader)
        {
            if (dataGridColumnHeader != null && dataGridColumnHeader.Column != null && this.Model.ColumnPool.Layout.ItemsSource != null)
            {
                var visibleColumnsCount = this.Model.ColumnPool.Layout.ItemsSource.Count;

                if (dataGridColumnHeader.Column.ItemInfo.Slot == visibleColumnsCount - 1 && this.ColumnDataOperationsMode == ColumnDataOperationsMode.Flyout && this.CanUserChooseColumns)
                {
                    this.ColumnReorderServicePanel.Visibility = dataGridColumnHeader.IsSelected ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        private static void OnAutoGenerateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.model.AutoGenerateColumns = (bool)e.NewValue;
        }

        private static void OnFrozenColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;

            grid.CancelEdit();
            grid.model.FrozenColumnCount = (int)e.NewValue;
        }

        private static void OnCanUserChooseColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            if (grid != null && grid.IsTemplateApplied)
            {
                grid.ContentFlyout.Hide(DataGridFlyoutId.ColumnChooser);
                grid.UpdateColumnChooserButtonVisibility((bool)e.NewValue);
                grid.updateService.RegisterUpdate(UpdateFlags.AffectsContent);
            }
        }

        private static void OnColumnDataOperationsModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            if (grid != null && grid.IsTemplateApplied)
            {
                grid.ContentFlyout.Hide(DataGridFlyoutId.ColumnHeader);
                grid.UpdateColumnChooserButtonVisibility(grid.CanUserChooseColumns);
                grid.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }

        private void UpdateColumnChooserButtonVisibility(bool isVisible)
        {
            if (this.IsTemplateApplied)
            {
                this.ClearColumnHeaderSelection();

                this.ColumnReorderServicePanel.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ClearColumnHeaderSelection()
        {
            if (this.LastSelectedColumn != null)
            {
                if (this.LastSelectedColumn.HeaderControl != null && this.ColumnDataOperationsMode == ColumnDataOperationsMode.Flyout)
                {
                    var context = this.GenerateColumnHeaderTapContext(this.LastSelectedColumn, Windows.Devices.Input.PointerDeviceType.Mouse);

                    this.CommandService.ExecuteDefaultCommand(Grid.Commands.CommandId.ColumnHeaderTap, context);
                }

                this.LastSelectedColumn = null;
            }
        }
    }
}