using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines a <see cref="DataGridColumn"/> which content is known (strongly typed) and represented by internally built UI. Typed columns are highly optimized for better performance.
    /// </summary>
    public abstract class DataGridTypedColumn : DataGridColumn
    {
        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(DataGridTypedColumn), new PropertyMetadata(null, OnPropertyNameChanged));

        /// <summary>
        /// Identifies the <see cref="CellContentStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellContentStyleProperty =
            DependencyProperty.Register(nameof(CellContentStyle), typeof(Style), typeof(DataGridTypedColumn), new PropertyMetadata(null, OnCellContentStyleChanged));

        /// <summary>
        /// Identifies the <see cref="CellContentStyleSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellContentStyleSelectorProperty =
            DependencyProperty.Register(nameof(CellContentStyleSelector), typeof(StyleSelector), typeof(DataGridTypedColumn), new PropertyMetadata(null, OnCellContentStyleSelectorChanged));

        /// <summary>
        /// Identifies the <see cref="CellEditorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellEditorStyleProperty =
            DependencyProperty.Register(nameof(CellEditorStyle), typeof(Style), typeof(DataGridTypedColumn), new PropertyMetadata(null, OnCellEditorStyleChanged));
        
        private IDataFieldInfo propertyInfo;

        private string propertyNameCache;
        private Style cellContentStyleCache;
        private Style cellEditorStyleCache;
        private StyleSelector cellContentStyleSelectorCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridTypedColumn" /> class.
        /// </summary>
        protected DataGridTypedColumn()
        {
        }

        /// <summary>
        /// Gets or sets the name of the property of the object type that represents each row within the grid.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyNameCache;
            }
            set
            {
                if (this.propertyNameCache == value)
                {
                    return;
                }

                this.SetValue(PropertyNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> object that defines the appearance of each cell associated with this column.
        /// The TargetType of the Style depends on the type of the column - e.g. for <see cref="DataGridTextColumn"/> it will be the <see cref="TextBlock"/> type.
        /// </summary>
        public Style CellContentStyle
        {
            get
            {
                return this.cellContentStyleCache;
            }
            set
            {
                this.SetValue(CellContentStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> instance that allows for dynamic appearance on a per cell basis.
        /// </summary>
        public StyleSelector CellContentStyleSelector
        {
            get
            {
                return this.cellContentStyleSelectorCache;
            }
            set
            {
                this.SetValue(CellContentStyleSelectorProperty, value);
            }
        }
        
        /// <summary>
        /// Gets or sets the <see cref="Style"/> that will be applied to the cell editor.
        /// </summary>
        public Style CellEditorStyle
        {
           get
           {
               return this.cellEditorStyleCache;
           }
           set
           {
               this.SetValue(CellEditorStyleProperty, value);
           }
        }

        internal virtual Style DefaultCellContentStyle
        {
            get
            {
                return null;
            }
        }

        internal virtual Style DefaultCellEditorStyle
        {
            get
            {
                return null;
            }
        }

        internal virtual Style DefaultCellFlyoutContentStyle
        {
            get
            {
                return null;
            }
        }

        internal bool PropertyInfoInitialized
        {
            get
            {
                return !string.IsNullOrEmpty(this.PropertyName) && this.PropertyInfo != null;
            }
        }

        internal IDataFieldInfo PropertyInfo
        {
            get
            {
                return this.propertyInfo;
            }
            set
            {
                if (this.propertyInfo == value)
                {
                    return;
                }

                this.propertyInfo = value;
                if (this.propertyInfo == null)
                {
                    return;
                }

                if (this.propertyNameCache != this.propertyInfo.Name)
                {
                    this.SetValue(PropertyNameProperty, this.propertyInfo.Name);
                }

                this.UpdateHeader();
            }
        }

        internal override bool CanGroupBy
        {
            get
            {
                if (this.Model != null &&
                    this.Model.GroupDescriptors.OfType<PropertyGroupDescriptor>().Any(c => c.PropertyName == this.PropertyName))
                {
                    return false;
                }

                return base.CanGroupBy;
            }
        }

        /// <summary>
        /// Retrieves the column value for the provided object instance. 
        /// This actually represents the content of a grid cell where a cell is defined by a row (data item) and a column.
        /// </summary>
        public override object GetValueForInstance(object instance)
        {
            IGroup group = instance as IGroup;
            if (group != null)
            {
                // TODO: Return group aggregates here ??
                return group.Name;
            }

            var memberAccess = this.propertyInfo as IMemberAccess;
            if (memberAccess != null)
            {
                return memberAccess.GetValue(instance);
            }

            return null;
        }

        /// <summary>
        /// Creates an instance of the editor used by the column when entering edit mode.
        /// </summary>
        /// <returns>An instance of the editor.</returns>
        public abstract FrameworkElement CreateEditorContentVisual();

        /// <summary>
        /// Prepares all bindings and content set to the editor visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        /// <param name="binding">The binding set to the editor of the cell.</param>
        public abstract void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding);

        /// <summary>
        /// Clears all bindings and content set to the editor visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public abstract void ClearEditorContentVisual(FrameworkElement editorContent);

        /// <summary>
        /// Prepares the UIElement inside which the cell is visualized.
        /// </summary>
        /// <param name="container">The element inside which the cell is visualized.</param>
        /// <param name="value">The value of the cell.</param>
        /// <param name="item">The business object represented in the cell.</param>
        public override void PrepareCell(object container, object value, object item)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element == null)
            {
                return;
            }

            Style style = this.ComposeCellContentStyle(element, item);
            if (style != null)
            {
                element.Style = style;
            }
            else
            {
                element.ClearValue(FrameworkElement.StyleProperty);
            }
        }

        internal override void SetValueForInstance(object instance, object value)
        {
            var memberAccess = this.propertyInfo as IMemberAccess;
            if (memberAccess != null)
            {
                memberAccess.SetValue(instance, value);
            }
        }

        internal override void Attach(GridModel model)
        {
            base.Attach(model);

            if (!model.columns.IsSuspended && model.FieldInfoData != null && model.FieldInfoData.RootFieldInfo != null)
            {
                this.PropertyInfo = model.FieldInfoData.GetFieldDescriptionByMember(this.PropertyName);
            }
        }

        internal override SortDescriptorBase GetSortDescriptor()
        {
            return this.CreateSortDescriptor();
        }

        internal override GroupDescriptorBase GetGroupDescriptor()
        {
            return this.CreateGroupDescriptor();
        }

        internal override object CreateEditorContainer(object rowItem, object containerType)
        {
            if (!this.CanEdit || containerType != this.GetEditorType(rowItem))
            {
                return this.CreateContainer(rowItem);
            }

            ////TODO: Create value proxy to provide option for delayed update of the values in the VM behind.
            FrameworkElement content = this.CreateEditorContentVisual();

            var host = new EditRowHostPanel();
            host.Owner = this.Model.GridView;
            var validation = new ValidationControl()
            {
                ControlPeer = content
            };

            host.Children.Add(content);
            host.Children.Add(validation);

            return new Tuple<FrameworkElement, FrameworkElement, FrameworkElement>(content, host, validation);
        }

        internal override void PrepareEditorContainer(GridCellEditorModel editor)
        {
            if (!this.CanEdit)
            {
                this.PrepareCell(editor);
                return;
            }

            var binding = new Binding()
            {
                Path = new PropertyPath(this.PropertyName),
                Mode = BindingMode.TwoWay,
                Source = editor.ParentRow.ItemInfo.Item
            };

            var pair = editor.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

            var validation = pair.Item3 as ValidationControl;

            if (validation != null)
            {
                validation.PropertyName = this.PropertyName;
                validation.DataItem = editor.ParentRow.ItemInfo.Item;
                validation.Validating += this.OnCellValidating;
            }

            var control = pair.Item1 as Control;
            if (control != null)
            {
                control.IsTabStop = true;
                control.TabIndex = this.CollectionIndex;
            }

            pair.Item1.Style = this.cellEditorStyleCache != null ? this.cellEditorStyleCache : this.DefaultCellEditorStyle;

            editor.EditorHost = pair.Item2;
            this.PrepareEditorContentVisual(pair.Item1, binding);
        }

        internal override void ClearEditorContainer(GridCellEditorModel editor)
        {
            if (!this.CanEdit)
            {
                return;
            }

            var pair = editor.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

            var control = pair.Item1 as Control;
            if (control != null)
            {
                control.IsTabStop = false;
            }

            this.ClearEditorContentVisual(pair.Item1);

            var validation = pair.Item3 as ValidationControl;

            if (validation != null)
            {
                validation.Validating -= this.OnCellValidating;
            }
        }

        internal override bool IsAssociatedWithDescriptor(IPropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.PropertyName == this.PropertyName;
        }

        internal override bool IsCellValid(GridCellEditorModel editor)
        {
            return this.IsCellValid(editor.ParentRow.ItemInfo.Item);
        }

        internal override bool IsCellValid(object item)
        {
            var errors = new List<object>();

            // Cannot use validation control since the virtualized ones will not be validated.
            var dataErrorInfo = item as INotifyDataErrorInfo;

            if (dataErrorInfo != null)
            {
                errors = dataErrorInfo.GetErrors(this.PropertyName).OfType<object>().ToList();
            }

            var asyncErrorInfo = item as IAsyncDataErrorInfo;

            if (asyncErrorInfo != null)
            {
                asyncErrorInfo.ValidateAsync(this.PropertyName).Wait();
                errors = dataErrorInfo.GetErrors(this.PropertyName).OfType<object>().ToList();
            }

            var grid = this.Model.GridView as RadDataGrid;

            if (grid != null)
            {
                this.RaiseValidateCommands(grid, new ValidateCellContext(errors, new DataGridCellInfo(item, this)));
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Creates the <see cref="GroupDescriptorBase"/> instance that is used to group by this column through the user interface.
        /// </summary>
        protected internal virtual GroupDescriptorBase CreateGroupDescriptor()
        {
            var descriptor = new PropertyGroupDescriptor() { PropertyName = this.PropertyName };
            if (this.IsUserHeader && !(this.Header is FrameworkElement))
            {
                descriptor.DisplayContent = this.Header;
            }

            return descriptor;
        }

        /// <summary>
        /// Creates the <see cref="SortDescriptorBase"/> instance that is used to sort by this column through the user interface.
        /// </summary>
        protected virtual SortDescriptorBase CreateSortDescriptor()
        {
            return new PropertySortDescriptor() { PropertyName = this.PropertyName };
        }

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridTypedColumn definition = d as DataGridTypedColumn;
            definition.propertyNameCache = e.NewValue as string;

            // Assuming that for the time being the path is property name (Simple Binding).
            definition.UpdatePropertyInfo(definition.propertyNameCache);

            definition.OnPropertyChange(UpdateFlags.All);
        }

        private static void OnCellContentStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as DataGridTypedColumn;
            definition.cellContentStyleCache = e.NewValue as Style;
            definition.OnPropertyChange(UpdateFlags.AllButData);
        }

        private static void OnCellContentStyleSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as DataGridTypedColumn;
            definition.cellContentStyleSelectorCache = e.NewValue as StyleSelector;
            definition.OnPropertyChange(UpdateFlags.AllButData);
        }
        
        private static void OnCellEditorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as DataGridTypedColumn;
            definition.cellEditorStyleCache = e.NewValue as Style;
            definition.OnPropertyChange(UpdateFlags.AllButData);
        }

        private Style ComposeCellContentStyle(FrameworkElement container, object item)
        {
            if (this.cellContentStyleCache != null)
            {
                return this.cellContentStyleCache;
            }

            if (this.cellContentStyleSelectorCache != null)
            {
                var selectContext = new DataGridCellInfo(item, this);
                var style = this.cellContentStyleSelectorCache.SelectStyle(selectContext, container);
                if (style != null)
                {
                    return style;
                }
            }

            return this.DefaultCellContentStyle;
        }

        private void UpdatePropertyInfo(string path)
        {
            if (this.propertyInfo == null)
            {
                return;
            }

            if (this.propertyInfo.Name != path)
            {
                if (this.Model != null && this.Model.FieldInfoData != null)
                {
                    this.propertyInfo = this.Model.FieldInfoData.GetFieldDescriptionByMember(this.PropertyName);
                }
                this.UpdateHeader();
            }
        }

        private void UpdateHeader()
        {
            if (!this.IsUserHeader)
            {
                this.ChangePropertyInternally(DataGridColumn.HeaderProperty, this.propertyNameCache);
            }
        }

        private void OnCellValidating(object sender, ValidatingEventArgs e)
        {
            var grid = this.Model.GridView as RadDataGrid;
            var validation = sender as ValidationControl;
            if (grid != null && validation != null)
            {
                this.RaiseValidateCommands(grid, new ValidateCellContext(e.Errors, new DataGridCellInfo(validation.DataItem, this)));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private void RaiseValidateCommands(RadDataGrid grid, ValidateCellContext context)
        {
            grid.CommandService.ExecuteCommand(CommandId.ValidateCell, context);
        }
    }
}
