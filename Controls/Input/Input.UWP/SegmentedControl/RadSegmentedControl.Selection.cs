using System;
using System.Collections.Generic;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// A segmented control is a horizontal control made of multiple segments, each segment functioning as a toggle button.
    /// </summary>
    public partial class RadSegmentedControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(RadSegmentedControl), new PropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RadSegmentedControl), new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedValuePath"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string), typeof(RadSegmentedControl), new PropertyMetadata(null, OnSelectedValuePathChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedValue"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(RadSegmentedControl), new PropertyMetadata(null, OnSelectedValueChanged));

        private object selectedItemCache;

        /// <summary>
        /// Occurs when the selection changes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Gets or sets the zero-based index of the currently selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { this.SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the first item in the current selection or returns null if the selection is empty.
        /// </summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="SelectedItem"/>, obtained by using <see cref="SelectedValuePath"/>.
        /// </summary>
        public object SelectedValue
        {
            get { return this.GetValue(SelectedValueProperty); }
            set { this.SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the path that is used to get the <see cref="SelectedValue"/> from the <see cref="SelectedItem"/>.
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { this.SetValue(SelectedValuePathProperty, value); }
        }

        internal void OnSegmentChecked(Segment segment)
        {
            this.isInternalChange = true;

            this.SelectedIndex = this.itemsControl.IndexFromContainer(segment);
            this.SelectedItem = this.itemsControl.ItemFromContainer(segment);
            this.SelectedValue = this.SelectedItem.GetPropertyValue(this.SelectedValuePath);

            this.isInternalChange = false;
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadSegmentedControl;

            if (!control.isInternalChange)
            {
                var selectedIndex = (int)e.NewValue;
                var oldSelectedIndex = (int)e.OldValue;

                if (control.IsTemplateApplied)
                {
                    if (selectedIndex == -1)
                    {
                        control.ClearSelection();
                        return;
                    }

                    var container = control.itemsControl.ContainerFromIndex(selectedIndex) as Segment;
                    if (container != null)
                    {
                        if (oldSelectedIndex >= 0)
                        {
                            var oldContainer = control.itemsControl.ContainerFromIndex(oldSelectedIndex) as Segment;
                            if (oldContainer != null)
                            {
                                oldContainer.IsSelected = false;
                            }
                        }

                        container.IsSelected = true;
                    }
                    else
                    {
                        try
                        {
                            throw new ArgumentException("Value does not fall within the expected range.");
                        }
                        finally
                        {
                            control.isInternalChange = true;
                            control.SelectedIndex = oldSelectedIndex;
                            control.isInternalChange = false;
                        }
                    }
                }
                else
                {
                    var item = control.GetItemByIndex(selectedIndex);
                    control.UpdateSelectedValue(item.GetPropertyValue(control.SelectedValuePath));
                }
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadSegmentedControl;

            bool isValid = true;

            if (!control.isInternalChange)
            {
                if (e.NewValue == null)
                {
                    control.ClearSelection();
                }
                else if (isValid = control.IsSelectedItemValid(e.NewValue))
                {
                    if (control.IsTemplateApplied)
                    {
                        var oldContainer = control.itemsControl.ContainerFromItem(e.OldValue) as Segment;
                        if (oldContainer != null)
                        {
                            oldContainer.IsSelected = false;
                        }

                        var newContainer = control.itemsControl.ContainerFromItem(e.NewValue) as Segment;
                        if (newContainer != null)
                        {
                            newContainer.IsSelected = true;
                        }
                        else
                        {
                            control.ClearSelection();
                        }
                    }
                    else
                    {
                        control.UpdateSelectedItem(e.NewValue);
                    }
                }
                else
                {
                    control.isInternalChange = true;
                    control.SelectedItem = e.OldValue;
                    control.isInternalChange = false;
                }
            }

            if (isValid &&
                ((e.NewValue == null && control.selectedItemCache != null) ||
                 (e.NewValue != null && !e.NewValue.Equals(control.selectedItemCache))))
            {
                control.selectedItemCache = e.NewValue;
                var oldItems = e.OldValue != null ? new List<object> { e.OldValue } : new List<object>();
                var newItems = control.SelectedItem != null ? new List<object> { control.SelectedItem } : new List<object>();

                control.OnSelectionChanged(oldItems, newItems);
            }
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadSegmentedControl;

            if (!control.isInternalChange)
            {
                var selectedValue = e.NewValue;

                if (control.IsTemplateApplied)
                {
                    var container = control.GetContainerForValue(selectedValue);

                    if (container != null)
                    {
                        container.IsSelected = true;
                    }
                    else
                    {
                        control.ClearSelection();
                    }
                }
                else
                {
                    control.UpdateSelectedValue(selectedValue);
                }
            }
        }

        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadSegmentedControl;
            if (control.IsTemplateApplied)
            {
                control.itemsControl.PrepareContainers();
            }

            control.UpdateSelectedValue(control.SelectedValue);
        }

        private void OnSelectionChanged(IList<object> oldItems, IList<object> newItems)
        {
            var handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs(oldItems, newItems));
            }
        }

        private void ClearSelection()
        {
            this.isInternalChange = true;

            this.SelectedValue = null;
            this.SelectedItem = null;
            this.SelectedIndex = -1;

            this.isInternalChange = false;

            if (this.IsTemplateApplied)
            {
                this.itemsControl.ClearSelectedContainer();
            }
        }

        private bool IsSelectedItemValid(object selectedItem)
        {
            var source = this.IsTemplateApplied ? this.itemsControl.Items : this.ItemsSource ?? this.Items;

            foreach (var item in source)
            {
                if ((item == null && selectedItem == null) || (selectedItem != null && selectedItem.Equals(item)))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateSelectedItem(object selectedItem)
        {
            if (this.IsSelectedItemValid(selectedItem))
            {
                this.isInternalChange = true;

                this.SelectedIndex = this.GetIndexByItem(selectedItem);
                this.SelectedItem = selectedItem;
                this.SelectedValue = selectedItem.GetPropertyValue(this.SelectedValuePath);

                this.isInternalChange = false;
            }
            else
            {
                this.ClearSelection();
            }
        }

        private void UpdateSelectedValue(object selectedValue)
        {
            object selectedItem;

            if (this.TryGetItemByValue(selectedValue, out selectedItem))
            {
                this.isInternalChange = true;

                this.SelectedIndex = this.GetIndexByItem(selectedItem);
                this.SelectedItem = selectedItem;
                this.SelectedValue = selectedItem.GetPropertyValue(this.SelectedValuePath);

                this.isInternalChange = false;
            }
            else
            {
                this.ClearSelection();
            }
        }
    }
}