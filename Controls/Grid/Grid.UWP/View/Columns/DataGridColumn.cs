using System;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines an abstraction of a table column that is used to visualize data within a <see cref="RadDataGrid"/> component.
    /// A column generally represents a Property within the underlying ViewModel.
    /// </summary>
    public abstract partial class DataGridColumn : RadDependencyObject, IDataDescriptorPeer
    {
        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(DataGridColumn), new PropertyMetadata(100d, OnWidthChanged));

        /// <summary>
        /// Identifies the <see cref="SizeMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeModeProperty =
            DependencyProperty.Register(nameof(SizeMode), typeof(DataGridColumnSizeMode), typeof(DataGridColumn), new PropertyMetadata(DataGridColumnSizeMode.Stretch, OnSizeModeChanged));

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(DataGridColumn), new PropertyMetadata(null, OnHeaderChanged));

        /// <summary>
        /// Identifies the <see cref="SortDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register(nameof(SortDirection), typeof(SortDescription), typeof(DataGridColumn), new PropertyMetadata(SortDirection.None));

        /// <summary>
        /// Identifies the <see cref="HeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(DataGridColumn), new PropertyMetadata(null, OnHeaderStyleChanged));

        /// <summary>
        /// Identifies the <see cref="CanUserGroup"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserGroupProperty =
            DependencyProperty.Register(nameof(CanUserGroup), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanUserFilter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserFilterProperty =
            DependencyProperty.Register(nameof(CanUserFilter), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true, OnCanUserFilterChanged));

        /// <summary>
        /// Identifies the <see cref="CanUserSort"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserSortProperty =
            DependencyProperty.Register(nameof(CanUserSort), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true, OnCanUserSortChanged));

        /// <summary>
        /// Identifies the <see cref="CanUserResize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserResizeProperty =
            DependencyProperty.Register(nameof(CanUserResize), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanUserReorder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserReorderProperty =
            DependencyProperty.Register(nameof(CanUserReorder), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CanUserEdit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserEditProperty =
            DependencyProperty.Register(nameof(CanUserEdit), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Name"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register(nameof(Name), typeof(string), typeof(DataGridColumn), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DataOperationsFlyoutTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataOperationsFlyoutTemplateProperty =
            DependencyProperty.Register(nameof(DataOperationsFlyoutTemplate), typeof(DataTemplate), typeof(DataGridColumn), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register(nameof(IsVisible), typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true, OnIsVisibleChanged));

        internal static double DefaultWidth = 100;
        internal int CollectionIndex;
        
        private Style headerStyleCache;
        private object headerCache;
        private bool isUserHeader;
        private bool isFiltered;
        private bool canSortCache;
        private bool updatingSort;
        private double widthCache = DefaultWidth;
        private double actualWidth;
        private SortDescriptorBase currentSortDescriptor;
        private DataGridColumnSizeMode sizeModeCache = DataGridColumnSizeMode.Stretch;
        private WeakReference<DataGridColumnHeader> headerControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumn" /> class.
        /// </summary>
        protected DataGridColumn()
        {
            this.canSortCache = true;
            this.CollectionIndex = -1;
        }
        
        /// <summary>
        /// Gets or sets the unique name of the column. Typically this is used as an identifier for this particular instance.
        /// <remarks>
        /// This value is used by the string indexer in the <see cref="RadDataGrid.Columns"/> collection.
        /// </remarks>
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikGrid:RadDataGrid x:Name="grid"&gt;
        ///   &lt;telerikGrid:RadDataGrid.Columns&gt;
        ///     &lt;telerikGrid:DataGridTextColumn Name="FirstColumn"/&gt;
        ///   &lt;/telerikGrid:RadDataGrid.Columns&gt;
        /// &lt;/telerikGrid:RadDataGrid&gt;
        /// </code>
        /// <code language="c#">
        ///  this.grid.Columns[0].Name = "FirstColumn";
        /// </code>
        /// </example>
        public string Name
        {
            get
            {
                return (string)this.GetValue(NameProperty);
            }
            set
            {
                this.SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can edit this column.
        /// </summary>
        /// <remarks>
        /// To enable/disable editing in RadDataGrid see the <see cref="P:Telerik.UI.Xaml.Controls.Grid.UserEditMode"/>
        /// </remarks>
        /// <value>
        /// The default value is "true".
        /// </value>
        public bool CanUserEdit
        {
            get { return (bool)this.GetValue(CanUserEditProperty); }
            set { this.SetValue(CanUserEditProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can resize this column.
        /// </summary>
        ///  /// <remarks>
        /// To enable/disable editing in RadDataGrid see the <see cref="P:Telerik.UI.Xaml.Controls.Grid.UserColumnResizeMode"/>
        /// </remarks>
        /// <value>
        /// The default value is "true".
        /// </value>
        public bool CanUserResize
        {
            get { return (bool)this.GetValue(CanUserResizeProperty); }
            set { this.SetValue(CanUserResizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can filter this column by using the built-in filtering UI.
        /// </summary>
        /// <remarks>
        /// To enable/disable Filtering in RadDataGrid see the <see cref="P:Telerik.UI.Xaml.Controls.Grid.UserFilterMode"/>
        /// </remarks>
        /// <value>
        /// The default value is "true".
        /// </value>
        public bool CanUserFilter
        {
            get
            {
                return (bool)this.GetValue(CanUserFilterProperty);
            }
            set
            {
                this.SetValue(CanUserFilterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can reorder the column.
        /// </summary>
        /// <remarks>
        /// To enable/disable column reordering see the <see cref="P:Telerik.UI.Xaml.Controls.Grid.UserColumnReorderMode"/>
        /// </remarks>
        /// <value>
        /// The default value is "true".
        /// </value>
        public bool CanUserReorder
        {
            get { return (bool)this.GetValue(CanUserReorderProperty); }
            set { this.SetValue(CanUserReorderProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can group-by this column by using the built-in Grouping UI.
        /// </summary>
        /// <remarks>
        /// To enable/disable grouping in RadDataGrid see the <see cref="P:Telerik.UI.Xaml.Controls.Grid.UserGroupMode"/>.
        /// </remarks>
        /// <value>
        /// The default value is "true".
        /// </value>
        public bool CanUserGroup
        {
            get { return (bool)this.GetValue(CanUserGroupProperty); }
            set { this.SetValue(CanUserGroupProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can sort the data by the values in this column.
        /// </summary>
        /// <remarks>
        /// To enable/disable sorting in RadDataGrid see the <see cref="P:Telerik.UI.Xaml.Controls.Grid.UserSortMode"/>.
        /// </remarks>
        /// <value>The default value is "true".</value>
        public bool CanUserSort
        {
            get { return (bool)this.GetValue(CanUserSortProperty); }
            set { this.SetValue(CanUserSortProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if the current column is visible or not.
        /// </summary>   
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { this.SetValue(IsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that defines the appearance of the <see cref="DataGridColumnHeader"/> control.
        /// </summary>   
        public Style HeaderStyle
        {
            get
            {
                return this.headerStyleCache;
            }
            set
            {
                this.SetValue(HeaderStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataGridColumnSizeMode"/> value that controls how the column and its associated cells are sized horizontally.
        /// </summary>
        public DataGridColumnSizeMode SizeMode
        {
            get
            {
                return this.sizeModeCache;
            }
            set
            {
                this.SetValue(SizeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the fixed width for the column. Applicable when the SizeMode property is set to DataGridColumnSizeMode.Fixed.
        /// </summary>
        public double Width
        {
            get
            {
                return this.widthCache;
            }
            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual width of the column.
        /// </summary>
        public double ActualWidth
        {
            get
            {
                return this.actualWidth;
            }
            internal set
            {
                this.actualWidth = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column is auto-generated internally.
        /// </summary>
        public bool IsAutoGenerated
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the content to be displayed in the Header UI that represents the column.
        /// </summary>
        public object Header
        {
            get
            {
                return this.headerCache;
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="SortDirection"/> value for the column.
        /// This property is used for visualization purposes only and does not affect the actual sorting state of the owning <see cref="RadDataGrid"/> component.
        /// </summary>
        public SortDirection SortDirection
        {
            get
            {
                return (SortDirection)this.GetValue(SortDirectionProperty);
            }
            set
            {
                this.SetValue(SortDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate for the DataOperations' flyout.
        /// </summary>
        public DataTemplate DataOperationsFlyoutTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DataOperationsFlyoutTemplateProperty);
            }
            set
            {
                this.SetValue(DataOperationsFlyoutTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the automatic width of the column - that is the largest cell (or header) width.
        /// This property is used to properly stretch (or shrink) columns with SizeMode.Stretch.
        /// </summary>
        internal double AutoWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the column before any Inflate calculations are applied (grid lines-related).
        /// </summary>
        internal double LayoutWidth
        {
            get;
            set;
        }
        
        internal bool IsFiltered
        {
            get
            {
                return this.isFiltered;
            }
        }

        /// <summary>
        /// Gets or sets a reference to the associated ItemInfo coming from the layouts.
        /// </summary>
        internal ItemInfo ItemInfo
        {
            get;
            set;
        }

        internal bool IsFrozen
        {
            get
            {
                if (this.Model == null)
                {
                    return false;
                }

                return this.CollectionIndex < this.Model.FrozenColumnCount;
            }
        }

        internal bool IsUserHeader
        {
            get
            {
                return this.isUserHeader;
            }
        }

        internal GridModel Model
        {
            get;
            private set;
        }

        internal virtual bool CanSort
        {
            get
            {
                return this.canSortCache;
            }
        }

        internal virtual bool CanFilter
        {
            get
            {
                if (!this.CanUserFilter)
                {
                    return false;
                }

                if (this.Model == null)
                {
                    return false;
                }

                var grid = this.Model.GridView as RadDataGrid;
                if (grid != null)
                {
                    return grid.UserFilterMode != DataGridUserFilterMode.Disabled;
                }

                return true;
            }
        }

        internal virtual bool CanGroupBy
        {
            get
            {
                if (!this.CanUserGroup)
                {
                    return false;
                }

                if (this.Model == null)
                {
                    return false;
                }

                var grid = this.Model.GridView as RadDataGrid;
                if (grid != null)
                {
                    return grid.UserGroupMode != DataGridUserGroupMode.Disabled;
                }

                return true;
            }
        }

        internal virtual bool SupportsCompositeFilter
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Method that updates the VisualState of the Header after filtering.
        /// </summary>
        public void UpdateFilterVisualState(bool isFiltered)
        {
            if (this.HeaderControl != null)
            {
                this.HeaderControl.IsFiltered = isFiltered;
                this.HeaderControl.Owner.updateService.RegisterUpdate((int)UpdateFlags.AffectsDecorations);
            }
        }

        bool IDataDescriptorPeer.IsAssociatedWithDescriptor(IPropertyDescriptor descriptor)
        {
            return this.IsAssociatedWithDescriptor(descriptor);
        }

        void IDataDescriptorPeer.OnDescriptorAssociated(DataDescriptor descriptor)
        {
            this.OnDescriptorAssociated(descriptor);
        }

        void IDataDescriptorPeer.OnAssociatedDescriptorPropertyChanged(DataDescriptor descriptor, string propertyName)
        {
            this.OnAssociatedDescriptorPropertyChanged(descriptor, propertyName);
        }

        void IDataDescriptorPeer.OnAssociatedDescriptorRemoved(DataDescriptor descriptor)
        {
            this.OnAssociatedDescriptorRemoved(descriptor);
        }

        /// <summary>
        /// Retrieves the column value for the provided object instance. 
        /// This actually represents the content of a grid cell where a cell is defined by a row (data item) and a column.
        /// </summary>
        public virtual object GetValueForInstance(object instance)
        {
            return null;
        }

        internal virtual void SetValueForInstance(object instance, object value)
        {
        }

        internal virtual SortDescriptorBase GetSortDescriptor()
        {
            return null;
        }

        internal virtual GroupDescriptorBase GetGroupDescriptor()
        {
            return null;
        }

        internal void OnProperyChange(UpdateFlags flags)
        {
            if (this.Model != null)
            {
                var update = new Update<UpdateFlags> { Sender = this, Flags = flags };
                this.Model.GridView.UpdateService.RegisterUpdate(update);
            }
        }

        internal bool ShouldRefreshCell(GridCellModel model)
        {
            var currentUpdate = this.Model.GridView.UpdateService.CurrentExecutingUpdate;
            bool contentUpdated = currentUpdate != null && currentUpdate.Sender == this && currentUpdate.Flags.HasFlag(UpdateFlags.AffectsContent);

            return contentUpdated || model is GridCellEditorModel;
        }

        internal virtual void Attach(GridModel model)
        {
            this.Model = model;
        }

        internal virtual void Detach()
        {
            this.Model = null;
        }

        internal virtual void OnDescriptorAssociated(DataDescriptor descriptor)
        {
            if (descriptor is FilterDescriptorBase)
            {
                this.isFiltered = true;

                this.UpdateFilterVisualState(this.isFiltered);
            }
            else if (descriptor != this.currentSortDescriptor)
            {
                var sortDescriptor = descriptor as SortDescriptorBase;
                if (sortDescriptor != null)
                {
                    this.currentSortDescriptor = sortDescriptor;
                    this.SortDirection = this.currentSortDescriptor.SortOrder == SortOrder.Ascending ? SortDirection.Ascending : SortDirection.Descending;
                }
            }
        }

        internal virtual void OnAssociatedDescriptorRemoved(DataDescriptor descriptor)
        {
            if (this.Model == null)
            {
                return;
            }

            if (descriptor is FilterDescriptorBase)
            {
                this.isFiltered = this.Model.FilterDescriptors.Where(d => d.DescriptorPeer == this).FirstOrDefault() != descriptor;

                this.UpdateFilterVisualState(this.isFiltered);
            }
            else if (!this.updatingSort && descriptor == this.currentSortDescriptor)
            {
                this.ClearSort();
            }
        }

        internal virtual void OnAssociatedDescriptorPropertyChanged(DataDescriptor dataDescriptor, string propertyName)
        {
            if (dataDescriptor == this.currentSortDescriptor && propertyName == "SortOrder")
            {
                // update the SortDirection respectively
                this.SortDirection = this.currentSortDescriptor.SortOrder == SortOrder.Ascending ? SortDirection.Ascending : SortDirection.Descending;
            }
        }

        internal virtual void ToggleSort(bool allowMultiple)
        {
            if (!this.CanSort)
            {
                this.ClearSort();
                return;
            }

            this.updatingSort = true;

            if (!allowMultiple)
            {
                this.Model.SortDescriptors.Clear();
            }

            if (this.currentSortDescriptor == null)
            {
                this.currentSortDescriptor = this.GetSortDescriptor();
                if (this.currentSortDescriptor == null)
                {
                    // Do nothing, we have an inheritor that returns null as a SortDescriptor.
                    return;
                }

                this.currentSortDescriptor.SortOrder = SortOrder.Ascending;
                this.Model.SortDescriptors.Add(this.currentSortDescriptor);
                this.SortDirection = SortDirection.Ascending;
            }
            else if (this.SortDirection == SortDirection.Ascending)
            {
                this.currentSortDescriptor.SortOrder = SortOrder.Descending;
                if (!this.Model.SortDescriptors.Contains(this.currentSortDescriptor))
                {
                    this.Model.SortDescriptors.Add(this.currentSortDescriptor);
                }
                this.SortDirection = SortDirection.Descending;
            }
            else
            {
                this.Model.SortDescriptors.Remove(this.currentSortDescriptor);
                this.currentSortDescriptor = null;
                this.SortDirection = SortDirection.None;
            }

            this.updatingSort = false;

            this.UpdateHeaderControlSortDirection();
        }

        internal virtual void ClearSort()
        {
            this.currentSortDescriptor = null;
            this.SortDirection = SortDirection.None;

            this.UpdateHeaderControlSortDirection();
        }

        internal abstract bool IsAssociatedWithDescriptor(IPropertyDescriptor propertyDescriptor);

        /// <summary>
        /// Creates the appropriate <see cref="DataGridFilterControlBase"/> instance that allows filtering operation to be applied upon this column.
        /// </summary>
        protected internal abstract DataGridFilterControlBase CreateFilterControl();

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;
            column.Model.OnColumnsCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;
            column.headerCache = e.NewValue;

            if (column.IsInternalPropertyChange)
            {
                return;
            }

            column.isUserHeader = column.headerCache != null;
        }

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;

            column.widthCache = (double)e.NewValue;
            column.OnProperyChange(UpdateFlags.AllButData);
        }

        private static void OnSizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;
            column.sizeModeCache = (DataGridColumnSizeMode)e.NewValue;

            if (column.sizeModeCache != DataGridColumnSizeMode.Fixed)
            {
                column.AutoWidth = 0;
            }

            column.OnProperyChange(UpdateFlags.AllButData);
        }

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as DataGridColumn;
            definition.headerStyleCache = e.NewValue as Style;
            definition.OnProperyChange(UpdateFlags.AllButData);
        }

        private static void OnCanUserFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;

            // TODO: possible optimization
            column.OnProperyChange(UpdateFlags.AllButData);
        }

        private static void OnCanUserSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;
            column.canSortCache = (bool)e.NewValue;
        }

        private void UpdateHeaderControlSortDirection()
        {
            var control = this.HeaderControl;
            if (control != null)
            {
                control.SortDirection = this.SortDirection;
            }
        }
    }
}
