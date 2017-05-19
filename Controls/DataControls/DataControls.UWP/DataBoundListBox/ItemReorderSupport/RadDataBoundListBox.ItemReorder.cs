using System;
using Telerik.UI.Xaml.Controls.Data.DataBoundListBox;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadDataBoundListBox
    {
        /// <summary>
        /// Identifies the <see cref="IsItemReorderEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsItemReorderEnabledProperty =
            DependencyProperty.Register(nameof(IsItemReorderEnabled), typeof(bool), typeof(RadDataBoundListBox), new PropertyMetadata(null, OnIsItemReorderEnabledChanged));

        /// <summary>
        /// Identifies the <see cref="ItemReorderControlStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemReorderControlStyleProperty =
            DependencyProperty.Register(nameof(ItemReorderControlStyle), typeof(Style), typeof(RadDataBoundListBox), new PropertyMetadata(null));

        internal DispatcherTimer holdTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
        internal bool isItemReorderEnabledCache = false;
        internal ItemReorderControl itemReorderControl;
        internal Popup itemReorderPopup;

        /// <summary>
        /// Fired when the end user taps on the Item Reorder button which shifts
        /// the target item up in the source collection.
        /// </summary>
        public event EventHandler ItemReorderUpButtonTap;

        /// <summary>
        /// Fired when the end user taps on the Item Reorder button which shifts
        /// the target item down in the source collection.
        /// </summary>
        public event EventHandler ItemReorderDownButtonTap;

        /// <summary>
        /// Fired when the Item Reorder functionality is either activated or deactivated.
        /// </summary>
        public event EventHandler<ItemReorderStateChangedEventArgs> ItemReorderStateChanged;

        /// <summary>
        /// Gets or sets an instance of the <see cref="Style"/> class
        /// that defines the visual appearance of the item reorder control.
        /// </summary>
        public Style ItemReorderControlStyle
        {
            get
            {
                return this.GetValue(ItemReorderControlStyleProperty) as Style;
            }
            set
            {
                this.SetValue(ItemReorderControlStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Item Reorder
        /// feature in <see cref="RadDataBoundListBox"/> is enabled.
        /// </summary>
        public bool IsItemReorderEnabled
        {
            get
            {
                return this.isItemReorderEnabledCache;
            }
            set
            {
                this.SetValue(IsItemReorderEnabledProperty, value);
            }
        }

        internal virtual bool IsItemReorderSupported
        {
            get
            {
                if (this.virtualizationStrategy == null)
                {
                    return true;
                }

                if (!(this.virtualizationStrategy is StackVirtualizationStrategy))
                {
                    return false;
                }

                if (this.listSource != null && this.listSource.SourceCollection != null)
                {
                    return this.listSource.SupportsItemReorder;
                }

                return true;
            }
        }

        /// <summary>
        /// Toggles the Item Reorder popup on or off according to the argument's value.
        /// </summary>
        /// <param name="pivotItem">The visual container which will be used as a pivot item to start the reordering.</param>
        public void ActivateItemReorderForItem(RadDataBoundListBoxItem pivotItem)
        {
            if (!this.IsProperlyTemplated)
            {
                return;
            }

            if (!this.IsItemReorderSupported)
            {
                return;
            }

            if (this.itemReorderPopup.IsOpen)
            {
                return;
            }

            this.StartItemReorderForItem(pivotItem);
        }

        internal void OnItemReorderMoveUpButtonTap()
        {
            EventHandler eh = this.ItemReorderUpButtonTap;

            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        internal void OnItemReorderMoveDownButtonTap()
        {
            EventHandler eh = this.ItemReorderDownButtonTap;

            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        internal void HandleItemReorderItemStateChanged(object item, ItemState state)
        {
            if (!this.IsProperlyTemplated)
            {
                return;
            }

            if (!this.itemReorderPopup.IsOpen)
            {
                return;
            }

            if (state == ItemState.Recycling)
            {
                VisualStateManager.GoToState(this.GetContainerForItem(item), "ItemNotOrdered", false);
            }
            else if (state == ItemState.Realized)
            {
                VisualStateManager.GoToState(this.GetContainerForItem(item), "ItemOrdered", false);
            }
        }

        internal virtual void OnItemHold(RadDataBoundListBoxItem item, HoldingRoutedEventArgs args)
        {
            if (!this.isItemReorderEnabledCache || !this.IsItemReorderSupported)
            {
                return;
            }

            if (item.associatedDataItem.Value == null || this.listSource.FindItem(item.associatedDataItem.Value) == null)
            {
                return;
            }

            this.StartItemReorderForItem(item);
        }

        internal virtual void OnItemMouseHold(RadDataBoundListBoxItem item)
        {
            if (!this.isItemReorderEnabledCache || !this.IsItemReorderSupported)
            {
                return;
            }

            if (item.associatedDataItem.Value == null || this.listSource.FindItem(item.associatedDataItem.Value) == null)
            {
                return;
            }

            this.StartItemReorderForItem(item);
        }

        internal override void OnVirtualizationStrategyDefinitionChanged(DependencyPropertyChangedEventArgs args)
        {
            base.OnVirtualizationStrategyDefinitionChanged(args);

            if (args.NewValue is WrapVirtualizationStrategyDefinition)
            {
                this.IsItemReorderEnabled = false;
            }
        }

        internal void StartItemReorderForItem(RadDataBoundListBoxItem item)
        {
            this.UnsubscribeRendering();

            this.itemReorderControl.targetItem = item;

            foreach (RadVirtualizingDataControlItem container in this.realizedItems)
            {
                if (container != item)
                {
                    VisualStateManager.GoToState(container, "ItemOrdered", false);
                }
            }

            this.itemReorderPopup.IsOpen = true;
            this.itemReorderControl.EvaluateButtonsAvailability();
        }

        private static void OnIsItemReorderEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as RadDataBoundListBox).OnIsItemReorderEnabledChanged(args);
        }

        private void OnIsItemReorderEnabledChanged(DependencyPropertyChangedEventArgs args)
        {
            bool newValue = (bool)args.NewValue;
            if (newValue && !this.IsItemReorderSupported)
            {
                throw new InvalidOperationException("Item reorder is not supported when in Wrap Mode.");
            }
            this.isItemReorderEnabledCache = newValue;
        }

        private void OnItemReorderStateChanged(bool isActive, RadDataBoundListBoxItem targetItem)
        {
            EventHandler<ItemReorderStateChangedEventArgs> eh = this.ItemReorderStateChanged;

            if (eh != null)
            {
                eh(this, new ItemReorderStateChangedEventArgs(isActive, targetItem));
            }
        }

        private void OnItemReorderPopup_OpenedChanged(object sender, object e)
        {
            if (this.itemReorderPopup.IsOpen)
            {
                VisualStateManager.GoToState(this.itemReorderControl.targetItem, "ItemNotOrdered", false);
            }
            else
            {
                VisualStateManager.GoToState(this.itemReorderControl.targetItem, "ItemOrdered", false);

                foreach (RadVirtualizingDataControlItem container in this.realizedItems)
                {
                    VisualStateManager.GoToState(container, "ItemNotOrdered", false);
                }
            }

            this.OnItemReorderStateChanged(this.itemReorderPopup.IsOpen, this.itemReorderControl.targetItem as RadDataBoundListBoxItem);
        }
    }
}
