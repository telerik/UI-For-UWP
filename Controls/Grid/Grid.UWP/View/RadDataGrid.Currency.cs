using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid : ICurrencyService
    {
        /// <summary>
        /// Identifies the <see cref="CurrentItem"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register(nameof(CurrentItem), typeof(object), typeof(RadDataGrid), new PropertyMetadata(null, OnCurrentItemChanged));

        /// <summary>
        /// Identifies the <see cref="EnsureCurrentItemIntoView"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EnsureCurrentItemIntoViewProperty =
            DependencyProperty.Register(nameof(EnsureCurrentItemIntoView), typeof(bool), typeof(RadDataGrid), new PropertyMetadata(true, OnEnsureCurrentItemIntoViewChanged));

        /// <summary>
        /// Identifies the <see cref="IsSynchronizedWithCurrentItem"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
            DependencyProperty.Register(nameof(IsSynchronizedWithCurrentItem), typeof(bool), typeof(RadDataGrid), new PropertyMetadata(false, OnIsSynchronizedWithCurrentItemChanged));

        internal readonly DataGridCurrencyService CurrencyService;

        /// <summary>
        /// Occurs when the <see cref="CurrentItem"/> property has changed.
        /// </summary>
        public event EventHandler CurrentItemChanged
        {
            add
            {
                this.CurrencyService.CurrentChanged += value;
            }
            remove
            {
                this.CurrencyService.CurrentChanged -= value;
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
                this.CurrencyService.CurrentChanging += value;
            }
            remove
            {
                this.CurrencyService.CurrentChanging -= value;
            }
        }

        /// <summary>
        /// Gets the object instance from the <see cref="ItemsSource"/> that is currently considered as Current.
        /// </summary>
        public object CurrentItem
        {
            get
            {
                return this.CurrencyService.CurrentItem;
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
                return this.CurrencyService.isSynchronizedWithCurrent;
            }
            set
            {
                this.SetValue(IsSynchronizedWithCurrentItemProperty, value);
            }
        }

        DataGridCurrencyService ICurrencyService.CurrencyService
        {
            get { return this.CurrencyService; }
        }

        private static void OnCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            if (!grid.IsInternalPropertyChange)
            {
                throw new InvalidOperationException("CurrentItem is read-only property.");
            }
        }

        private static void OnEnsureCurrentItemIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.CurrencyService.ensureCurrentIntoView = (bool)e.NewValue;
        }

        private static void OnIsSynchronizedWithCurrentItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            grid.CurrencyService.UpdateIsSynchronizedWithCurrent((bool)e.NewValue);
        }
    }
}
