using System;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public partial class RadListView
    {
        /// <summary>
        /// Identifies the <see cref="SelectionMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(DataControlsSelectionMode), typeof(RadListView), new PropertyMetadata(DataControlsSelectionMode.Single, OnSelectionModeChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RadListView), new PropertyMetadata(null, OnSelectedItemChanged));

        internal ListViewSelectionService selectionService;

        /// <summary>
        /// Occurs when the currently selected items change.
        /// </summary>
        public event EventHandler<ListViewSelectionChangedEventArgs> SelectionChanged
        {
            add
            {
                this.selectionService.SelectionChanged += value;
            }
            remove
            {
                this.selectionService.SelectionChanged -= value;
            }
        }

        /// <summary>
        /// Gets the currently selected items within this instance.      
        /// </summary>
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return this.selectionService.SelectedItems;
            }
        }

        /// <summary>
        /// Gets or sets the selected item of the <see cref="RadListView"/>.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return (object)this.GetValue(SelectedItemProperty);
            }
            set
            {
                this.SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection mode of the <see cref="RadListView"/>. The default value is <see cref="DataControlsSelectionMode.Single"/>.
        /// </summary>
        public DataControlsSelectionMode SelectionMode
        {
            get
            {
                return (DataControlsSelectionMode)this.GetValue(SelectionModeProperty);
            }
            set
            {
                this.SetValue(SelectionModeProperty, value);
            }
        }

        ListViewSelectionService ISelectionService.SelectionService
        {
            get
            {
                return this.selectionService;
            }
        }

        /// <summary>
        /// Selects the specified data item and adds it in the <see cref="SelectedItems"/> collection.
        /// </summary>
        public void SelectItem(object item)
        {
            this.selectionService.SelectItem(item, true, false);
        }

        /// <summary>
        /// Removes the selection for the specified data item and removes it from the <see cref="SelectedItems"/> collection.
        /// </summary>
        public void DeselectItem(object item)
        {
            this.selectionService.SelectItem(item, false, false);
        }

        /// <summary>
        /// Selects all the items as defined by the <see cref="SelectionMode"/> property.
        /// </summary>
        public void SelectAll()
        {
            this.selectionService.SelectAll();
        }

        /// <summary>
        /// Clears the current selection state.
        /// </summary>
        public void DeselectAll()
        {
            this.selectionService.ClearSelection();
            this.itemCheckBoxService.ClearSelection();
        }

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            var newValue = (DataControlsSelectionMode)e.NewValue;
            var oldValue = (DataControlsSelectionMode)e.OldValue;
#if WINDOWS_PHONE_APP
            if (newValue == DataControlsSelectionMode.MultipleWithCheckBoxes)
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += listView.HardwareButtons_BackPressed;
            }
            else
            {
                 Windows.Phone.UI.Input.HardwareButtons.BackPressed -= listView.HardwareButtons_BackPressed;
            }
#endif

            // Ensurance that itemscontrols bound to this property will not force incorrect refresh when the new and old value are same
            if (!oldValue.Equals(newValue))
            {
                listView.selectionService.OnSelectionModeChanged(newValue);
                if (newValue == DataControlsSelectionMode.MultipleWithCheckBoxes)
                {
                    listView.itemCheckBoxService.RefreshSelection();
                    if (listView.IsCheckModeActive)
                    {
                        listView.OnIsCheckModeActiveChanged(true);
                    }
                }
                if (oldValue == DataControlsSelectionMode.MultipleWithCheckBoxes && listView.IsCheckModeActive)
                {
                    listView.IsCheckModeActive = false;
                    listView.OnIsCheckModeActiveChanged(false, true);
                }
                listView.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
            }
        }

#if WINDOWS_PHONE_APP
                private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
                {
                    if (this.IsCheckModeActive)
                    {
                        this.IsCheckModeActive = false;
                        e.Handled = true;
                    } 
                }
#endif

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;

            if (!listView.IsInternalPropertyChange)
            {
                listView.selectionService.OnSelectedItemChanged(e.OldValue, e.NewValue);
                listView.currencyService.OnSelectedItemChanged(e.NewValue);
            }
        }
    }
}