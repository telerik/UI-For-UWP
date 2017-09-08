using System;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadListView
    {
        /// <summary>
        /// Identifies the <see cref="CurrentItem"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register(nameof(CurrentItem), typeof(object), typeof(RadListView), new PropertyMetadata(null, OnCurrentItemChanged));

        /// <summary>
        /// Identifies the <see cref="EnsureCurrentItemIntoView"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EnsureCurrentItemIntoViewProperty =
            DependencyProperty.Register(nameof(EnsureCurrentItemIntoView), typeof(bool), typeof(RadListView), new PropertyMetadata(true, OnEnsureCurrentItemIntoViewChanged));

        /// <summary>
        /// Identifies the <see cref="IsSynchronizedWithCurrentItem"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
            DependencyProperty.Register(nameof(IsSynchronizedWithCurrentItem), typeof(bool), typeof(RadListView), new PropertyMetadata(false, OnIsSynchronizedWithCurrentItemChanged));

        internal readonly ListViewCurrencyService currencyService;
        internal int currentLogicalIndex;

        /// <summary>
        /// Occurs when the <see cref="CurrentItem"/> property has changed.
        /// </summary>
        public event EventHandler CurrentItemChanged
        {
            add
            {
                this.currencyService.CurrentChanged += value;
            }
            remove
            {
                this.currencyService.CurrentChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="CurrentItem"/> property is about to change. The event may be canceled to prevent the actual change.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event CurrentChangingEventHandler CurrentItemChanging
        {
            add
            {
                this.currencyService.CurrentChanging += value;
            }
            remove
            {
                this.currencyService.CurrentChanging -= value;
            }
        }

        /// <summary>
        /// Gets the object instance from the <see cref="ItemsSource"/> that is currently considered as Current.
        /// </summary>
        public object CurrentItem
        {
            get
            {
                return this.currencyService.CurrentItem;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CurrentItem"/> should always be automatically scrolled into view.
        /// </summary>
        public bool EnsureCurrentItemIntoView
        {
            get
            {
                return (bool)this.GetValue(EnsureCurrentItemIntoViewProperty);
            }
            set
            {
                this.SetValue(EnsureCurrentItemIntoViewProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="SelectedItem"/> and <see cref="CurrentItem"/> are synchronized.
        /// </summary>
        public bool IsSynchronizedWithCurrentItem
        {
            get
            {
                return this.currencyService.isSynchronizedWithCurrent;
            }
            set
            {
                this.SetValue(IsSynchronizedWithCurrentItemProperty, value);
            }
        }

        ListViewCurrencyService ICurrencyService.CurrencyService
        {
            get { return this.currencyService; }
        }

        /// <summary>
        /// Tries to change the <see cref="CurrentItem"/> to the specified object. Returns value indicating if the operation is successful.
        /// </summary>
        public bool MoveCurrentTo(object item)
        {
            return this.currencyService.MoveCurrentTo(item);
        }

        internal void FocusCurrentContainer()
        {
            RadListViewItem container = this.GetCurrentContainer();
            if (container != null)
            {
                container.Focus(FocusState.Programmatic);
            }
        }
        
        private static void OnCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            if (!listView.IsInternalPropertyChange)
            {
                throw new InvalidOperationException("CurrentItem is read-only property.");
            }
        }

        private static void OnEnsureCurrentItemIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.currencyService.ensureCurrentIntoView = (bool)e.NewValue;
        }

        private static void OnIsSynchronizedWithCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;
            listView.currencyService.UpdateIsSynchronizedWithCurrent((bool)e.NewValue);
        }

        private RadListViewItem GetCurrentContainer()
        {
            if (this.CurrentItem == null || this.currencyService.CurrentItemInfo == null || !this.currencyService.CurrentItemInfo.HasValue)
            {
                return null;
            }

            ItemInfo currentInfo = this.currencyService.CurrentItemInfo.Value;
            GeneratedItemModel generatedModel = this.Model.GetDisplayedElement(currentInfo.Slot, currentInfo.Id);

            RadListViewItem result = null;
            if (generatedModel != null)
            {
                result = generatedModel.Container as RadListViewItem;
            }
            return result;
        }
    }
}
