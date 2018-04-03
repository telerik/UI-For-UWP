using System;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a <see cref="DataGridColumn"/> that uses a <see cref="DataTemplate"/> to describe the content of each associated grid cell.
    /// </summary>
    public class DataGridTemplateColumn : DataGridColumn
    {
        /// <summary>
        /// Identifies the <see cref="CellContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellContentTemplateProperty =
            DependencyProperty.Register(nameof(CellContentTemplate), typeof(DataTemplate), typeof(DataGridTemplateColumn), new PropertyMetadata(null, OnCellContentTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="CellContentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellContentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(CellContentTemplateSelector), typeof(DataTemplateSelector), typeof(DataGridTemplateColumn), new PropertyMetadata(null, OnCellContentTemplateSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="SortDescriptor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SortDescriptorProperty =
            DependencyProperty.Register(nameof(SortDescriptor), typeof(SortDescriptorBase), typeof(DataGridTemplateColumn), new PropertyMetadata(null, OnSortDescriptorChanged));

        /// <summary>
        /// Identifies the <see cref="GroupDescriptor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupDescriptorProperty =
            DependencyProperty.Register(nameof(GroupDescriptor), typeof(GroupDescriptorBase), typeof(DataGridTemplateColumn), new PropertyMetadata(null, OnGroupDescriptorChanged));

        private static readonly DataTemplate EmptyDataTemplate = new DataTemplate();

        private DataTemplate cellContentTemplateCache;
        private DataTemplateSelector cellContentTemplateSelectorCache;
        private SortDescriptorBase sortDescriptorCache;
        private GroupDescriptorBase groupDescriptorCache;

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance that defines the appearance of each cell associated with tis column.
        /// </summary>
        public DataTemplate CellContentTemplate
        {
            get
            {
                return this.cellContentTemplateCache;
            }
            set
            {
                if (this.cellContentTemplateCache == value)
                {
                    return;
                }

                this.SetValue(CellContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> instance that may be used to retrieve dynamic data templates on a per cell basis.
        /// </summary>
        public DataTemplateSelector CellContentTemplateSelector
        {
            get
            {
                return this.cellContentTemplateSelectorCache;
            }
            set
            {
                if (this.cellContentTemplateSelectorCache == value)
                {
                    return;
                }

                this.SetValue(CellContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SortDescriptorBase"/> that defines how the column will be sorted by the user upon a Tap gesture over the column header.
        /// </summary>
        public SortDescriptorBase SortDescriptor
        {
            get
            {
                return this.sortDescriptorCache;
            }
            set
            {
                this.SetValue(SortDescriptorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="GroupDescriptorBase"/> that defines whether the column may be grouped by the user using drag-and-drop operation.
        /// </summary>
        public GroupDescriptorBase GroupDescriptor
        {
            get
            {
                return this.groupDescriptorCache;
            }
            set
            {
                this.SetValue(GroupDescriptorProperty, value);
            }
        }

        internal override bool CanFilter
        {
            get
            {
                // TODO: Update when FilterDescriptor on a per Column basis added
                return false;
            }
        }

        internal override bool CanSort
        {
            get
            {
                return this.sortDescriptorCache != null;
            }
        }

        internal override bool CanGroupBy
        {
            get
            {
                if (this.groupDescriptorCache == null)
                {
                    return false;
                }

                return base.CanGroupBy;
            }
        }

        internal override bool CanEdit
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the column value for the provided object instance. 
        /// This actually represents the content of a grid cell where a cell is defined by a row (data item) and a column.
        /// </summary>
        public override object GetValueForInstance(object instance)
        {
            return instance;
        }

        /// <summary>
        /// Gets the type of the editor for the DataGridTemplateColumn that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public override object GetEditorType(object item)
        {
            return this.ComposeCellContentTemplate(item) ?? EmptyDataTemplate;
        }

        /// <summary>
        /// Gets the type of the control visualized when the template column is not currently edited.
        /// </summary>
        /// <returns>The type of the control.</returns>
        public override object GetContainerType(object rowItem)
        {
            return this.ComposeCellContentTemplate(rowItem) ?? EmptyDataTemplate;
        }
        
        /// <summary>
        /// Creates an instance of a FrameworkElement visualized when the column is not edited.
        /// </summary>
        /// <returns>An instance of the control.</returns>
        public override object CreateContainer(object rowItem)
        {
            var template = this.ComposeCellContentTemplate(rowItem);
            if (template == null)
            {
                return null;
            }

            var container = template.LoadContent() as FrameworkElement;
            if (container != null)
            {
                container.DataContext = rowItem;
            }

            return container;
        }

        /// <inheritdoc/>
        public override void PrepareCell(object container, object value, object item)
        {
            InvalidateCellUI(container);

            FrameworkElement cellContainer = container as FrameworkElement;
            if (cellContainer != null)
            {
                //// Force clear Selector.SelectedItem value when it is bound and data does not have value for selected item. 
                ////Clear dataContext to force bindings and fix issue with ExpandoObjects.
                //// if (container is Selector)

                cellContainer.DataContext = null;

                cellContainer.DataContext = value;
            }
        }

        internal override object CreateEditorContainer(object rowItem, object containerType)
        {
            var contrainer = this.CreateContainer(rowItem) as FrameworkElement;
            return new Tuple<FrameworkElement, FrameworkElement, FrameworkElement>(contrainer, contrainer, contrainer);
        }

        internal override void PrepareEditorContainer(GridCellEditorModel editor)
        {
            Tuple<FrameworkElement, FrameworkElement, FrameworkElement> pair = editor.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

            InvalidateCellUI(pair.Item2);

            if (pair.Item2 != null)
            {
                pair.Item2.DataContext = editor.Value;
            }
        }

        internal override void ClearEditorContainer(GridCellEditorModel editor)
        {
            Tuple<FrameworkElement, FrameworkElement, FrameworkElement> pair = editor.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

            var control = pair.Item2 as Control;
            if (control != null)
            {
                control.IsTabStop = false;
            }

            if (pair.Item2 != null)
            {
                pair.Item2.DataContext = null;
            }
        }

        internal override bool IsAssociatedWithDescriptor(IPropertyDescriptor propertyDescriptor)
        {
            return this.SortDescriptor == propertyDescriptor || this.GroupDescriptor == propertyDescriptor;
        }

        internal override RadSize MeasureEditCell(GridCellEditorModel cell, double availableWidth)
        {
            // This method is called by the XamlContentLayer
            RadSize size = RadSize.Empty;

            if (this.SizeMode == DataGridColumnSizeMode.Fixed)
            {
                availableWidth = this.Width;
            }

            var pair = cell.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;
            if (pair != null)
            {
                size = this.MeasureCellContainer(availableWidth, pair.Item1).ToRadSize();
            }

            if (this.SizeMode == DataGridColumnSizeMode.Fixed)
            {
                size.Width = this.Width;
            }

            return size;
        }

        internal override SortDescriptorBase GetSortDescriptor()
        {
            return this.sortDescriptorCache;
        }

        internal override GroupDescriptorBase GetGroupDescriptor()
        {
            return this.groupDescriptorCache;
        }

        internal override void TryFocusCell(DataGridCellInfo cellInfo, FocusState state)
        {
            base.TryFocusCell(cellInfo, state);

            if (cellInfo.Cell == null)
            {
                return;
            }

            var element = cellInfo.Cell.Container as FrameworkElement;
            if (element != null)
            {
                var control = element as Control;
                if (control != null)
                {
                    control.Focus(state);
                }
                else
                {
                    var firstControl = ElementTreeHelper.FindVisualDescendant<Control>(element);
                    if (firstControl != null)
                    {
                        firstControl.Focus(state);
                    }
                }
            }
        }

        /// <summary>
        /// There is no default built-in control that supports filtering over a Template column.
        /// </summary>
        protected internal override DataGridFilterControlBase CreateFilterControl()
        {
            return null;
        }

        private static void OnCellContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as DataGridTemplateColumn;

            definition.cellContentTemplateCache = e.NewValue as DataTemplate;
            definition.OnPropertyChange(UpdateFlags.AffectsContent);
        }

        private static void OnCellContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridTemplateColumn definition = d as DataGridTemplateColumn;

            definition.cellContentTemplateSelectorCache = e.NewValue as DataTemplateSelector;
            definition.OnPropertyChange(UpdateFlags.AffectsContent);
        }

        private static void OnSortDescriptorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridTemplateColumn;
            column.ChangeSortDescriptor(e.NewValue as SortDescriptorBase);
        }

        private static void OnGroupDescriptorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridTemplateColumn;
            column.ChangeGroupDescriptor(e.NewValue as GroupDescriptorBase);
        }

        private static void InvalidateCellUI(object container)
        {
            // This is needed to invalidate the availableSize field of RadChart, which does not have DesiredSize and builds its UI on the ArrangeOverride pass.
            var controlWithoutDesiredSize = container as INoDesiredSizeControl;
            if (controlWithoutDesiredSize != null)
            {
                controlWithoutDesiredSize.InvalidateUI();
            }
        }

        private DataTemplate ComposeCellContentTemplate(object rowItem)
        {
            if (this.cellContentTemplateCache != null)
            {
                return this.cellContentTemplateCache;
            }

            if (this.cellContentTemplateSelectorCache != null)
            {
                return this.cellContentTemplateSelectorCache.SelectTemplate(rowItem, null);
            }

            return null;
        }

        private void ChangeSortDescriptor(SortDescriptorBase newDescriptor)
        {
            if (this.Model != null && this.sortDescriptorCache != null)
            {
                this.Model.SortDescriptors.Remove(this.sortDescriptorCache);
            }

            this.sortDescriptorCache = newDescriptor;
        }

        private void ChangeGroupDescriptor(GroupDescriptorBase newDescriptor)
        {
            if (this.Model != null && this.groupDescriptorCache != null)
            {
                this.Model.GroupDescriptors.Remove(this.groupDescriptorCache);
            }

            this.groupDescriptorCache = newDescriptor;
        }
    }
}
