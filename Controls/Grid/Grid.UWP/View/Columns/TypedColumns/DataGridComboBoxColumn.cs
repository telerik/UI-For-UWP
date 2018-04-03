using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// A special <see cref="DataGridTextColumn"/> implementation which cell value editor is a <see cref="ComboBox"/> control.
    /// </summary>
    public class DataGridComboBoxColumn : DataGridTextColumn, INestedPropertyColumn
    {
        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(DataGridComboBoxColumn), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemsSourcePath"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ItemsSourcePathProperty =
            DependencyProperty.Register(nameof(ItemsSourcePath), typeof(string), typeof(DataGridComboBoxColumn), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedValuePath"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string), typeof(DataGridComboBoxColumn), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="DisplayMemberPath"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(DataGridComboBoxColumn), new PropertyMetadata(string.Empty, OnDisplayMemberPathChanged));

        private static readonly Type comboBoxType = typeof(ComboBox);

        private Style defaultCellEditorStyle;
        private Type itemsType;
        private Func<object, object> itemPropertyGetter;

        /// <summary>
        /// Gets or sets the items source for the <see cref="ComboBox"/> editor.
        /// </summary>
        public object ItemsSource
        {
            get
            {
                return this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path of the property that will be used as an
        /// items source for the <see cref="ComboBox"/> editor.
        /// </summary>
        public string ItemsSourcePath
        {
            get
            {
                return (string)this.GetValue(ItemsSourcePathProperty);
            }
            set
            {
                this.SetValue(ItemsSourcePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path of the property that is set as
        /// column value when an item is selected in the combobox editor.
        /// </summary>
        public string SelectedValuePath
        {
            get
            {
                return (string)this.GetValue(SelectedValuePathProperty);
            }
            set
            {
                this.SetValue(SelectedValuePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the display member path for the combobox editor.
        /// </summary>
        public string DisplayMemberPath
        {
            get
            {
                return (string)this.GetValue(DisplayMemberPathProperty);
            }
            set
            {
                this.SetValue(DisplayMemberPathProperty, value);
            }
        }

        internal override Style DefaultCellEditorStyle
        {
            get
            {
                if (this.defaultCellEditorStyle == null)
                {
                    this.defaultCellEditorStyle = ResourceHelper.LoadEmbeddedResource(
                        typeof(DataGridTextColumn),
                        "Telerik.UI.Xaml.Controls.Grid.View.Columns.Resources.DefaultComboBoxColumnEditorStyle.xaml",
                        "DefaultColumnEditorStyle") as Style;
                }

                return this.defaultCellEditorStyle;
            }
        }

        internal override bool CanGroupBy
        {
            get
            {
                if (this.Model != null &&
                    this.Model.GroupDescriptors.OfType<DelegateGroupDescriptor>().Any(d =>
                    {
                        var key = d.KeyLookup as NestedPropertyKeyLookup;
                        return key != null && key.DisplayValueGetter == this.itemPropertyGetter;
                    }))
                {
                    return false;
                }
                else
                {
                    return base.CanGroupBy;
                }
            }
        }

        /// <inheritdoc/>
        public object GetDisplayValueForInstance(object instance)
        {
            var item = this.GetValueForInstance(instance);

            if (string.IsNullOrEmpty(this.SelectedValuePath) && !string.IsNullOrEmpty(this.DisplayMemberPath))
            {
                var type = item.GetType();
                if (this.itemPropertyGetter == null)
                {
                    // TODO: does not work for dynamic objects
                    this.itemsType = type;
                    this.itemPropertyGetter = BindingExpressionHelper.CreateGetValueFunc(this.itemsType, this.DisplayMemberPath);
                }

                return this.itemPropertyGetter(item);
            }

            return item;
        }

        /// <summary>
        /// Gets the type of the editor for the DataGridComboBoxColumn that is visualized when entering in edit mode.
        /// </summary>
        /// <returns>The type of the editor.</returns>
        public override object GetEditorType(object item)
        {
            return this.CanEdit ? comboBoxType : DataGridNumericalColumn.TextBlockType;
        }

        /// <summary>
        /// Creates an instance of a ComboBox used by the column when entering edit mode.
        /// </summary>
        /// <returns>An instance of the editor.</returns>
        public override FrameworkElement CreateEditorContentVisual()
        {
            var comboBoxEditor = new ComboBox();
            comboBoxEditor.Unloaded += this.OnComboBoxUnloaded;
            comboBoxEditor.SelectionChanged += this.OnComboBoxSelectionChanged;
            return comboBoxEditor;
        }

        /// <summary>
        /// Prepares all bindings and content set to the ComboBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        /// <param name="binding">The binding set to the editor of the cell.</param>
        public override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            var item = binding.Source;

            var itemsSourceBinding = new Binding();
            if (!string.IsNullOrEmpty(this.ItemsSourcePath))
            {
                itemsSourceBinding.Source = item;
                itemsSourceBinding.Path = new PropertyPath(this.ItemsSourcePath);
            }
            else
            {
                itemsSourceBinding.Path = new PropertyPath("ItemsSource");
                itemsSourceBinding.Source = this;
            }

            var displayMemberPathBinding = new Binding
            {
                Path = new PropertyPath("DisplayMemberPath"),
                Source = this
            };

            var selectedValuePathBinding = new Binding
            {
                Path = new PropertyPath("SelectedValuePath"),
                Source = this
            };

            editorContent.SetBinding(ComboBox.ItemsSourceProperty, itemsSourceBinding);
            editorContent.SetBinding(ComboBox.DisplayMemberPathProperty, displayMemberPathBinding);
            editorContent.SetBinding(ComboBox.SelectedValuePathProperty, selectedValuePathBinding);

            if (!string.IsNullOrEmpty(this.SelectedValuePath))
            {
                var source = (editorContent as ComboBox).ItemsSource as IEnumerable<object>;

                if (source.Any())
                {
                    var comboBoxValueGetter = BindingExpressionHelper.CreateGetValueFunc(source.First().GetType(), this.SelectedValuePath);
                    var selectedItem = source.FirstOrDefault(x =>
                    {
                        var selectedItemValue = comboBoxValueGetter(x)?.Equals(this.GetValueForInstance(item));

                        return selectedItemValue ?? false;
                    });
                    
                    (editorContent as ComboBox).SelectedItem = selectedItem;
                    editorContent.SetBinding(ComboBox.SelectedValueProperty, binding);
                }
            }
            else
            {
                editorContent.SetBinding(ComboBox.SelectedItemProperty, binding);
            }
        }

        /// <summary>
        /// Clears all bindings and content set to the ComboBox visualized when entering edit mode.
        /// </summary>
        /// <param name="editorContent">The editor itself.</param>
        public override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
            if (!string.IsNullOrEmpty(this.SelectedValuePath))
            {
                editorContent.ClearValue(ComboBox.SelectedValueProperty);
                editorContent.ClearValue(ComboBox.SelectedItemProperty);
            }
            else
            {
                editorContent.ClearValue(ComboBox.SelectedItemProperty);
                editorContent.ClearValue(ComboBox.SelectedValueProperty);
            }

            editorContent.ClearValue(ComboBox.DisplayMemberPathProperty);
            editorContent.ClearValue(ComboBox.SelectedValuePathProperty);
            editorContent.ClearValue(ComboBox.ItemsSourceProperty);
        }

        /// <inheritdoc/>
        protected internal override DataGridFilterControlBase CreateFilterControl()
        {
            if (!string.IsNullOrEmpty(this.SelectedValuePath))
            {
                return base.CreateFilterControl();
            }
            else
            {
                return new DataGridNestedPropertyTextFilterControl { ItemPropertyGetter = this.itemPropertyGetter, PropertyName = this.PropertyName };
            }
        }

        /// <inheritdoc/>
        protected internal override GroupDescriptorBase CreateGroupDescriptor()
        {
            if (!string.IsNullOrEmpty(this.SelectedValuePath))
            {
                return base.CreateGroupDescriptor();
            }
            else
            {
                var descriptor = new DelegateGroupDescriptor
                {
                    KeyLookup = new NestedPropertyKeyLookup
                    {
                        DisplayValueGetter = this.itemPropertyGetter,
                        InstanceValueGetter = this.GetValueForInstance
                    }
                };

                if (this.IsUserHeader && !(this.Header is FrameworkElement))
                {
                    descriptor.DisplayContent = this.Header;
                }

                return descriptor;
            }
        }

        /// <inheritdoc/>
        protected override SortDescriptorBase CreateSortDescriptor()
        {
            if (!string.IsNullOrEmpty(this.SelectedValuePath))
            {
                return base.CreateSortDescriptor();
            }
            else
            {
                return new DelegateSortDescriptor
                {
                    KeyLookup = new NestedPropertyKeyLookup
                    {
                        DisplayValueGetter = this.itemPropertyGetter,
                        InstanceValueGetter = this.GetValueForInstance
                    }
                };
            }
        }

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridComboBoxColumn;
            if (column.itemsType != null)
            {
                column.itemPropertyGetter = BindingExpressionHelper.CreateGetValueFunc(column.itemsType, (string)e.NewValue);
                column.OnPropertyChange(UpdateFlags.All);
            }
        }

        private void OnComboBoxUnloaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.Unloaded -= this.OnComboBoxUnloaded;
                comboBox.SelectionChanged -= this.OnComboBoxSelectionChanged;
            }
        }

        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.InvalidateMeasure();
                comboBox.InvalidateArrange();
            }
        }
    }
}