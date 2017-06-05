using System.Collections;
using System.Collections.Specialized;
using Telerik.Core.Data;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Base class for data-bound controls.
    /// </summary>
    public abstract class DataControlBase : TemplateProviderControl, IListSourceProvider
    {
        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(DataControlBase), new PropertyMetadata(null, OnItemsSourceChanged));

        internal RadListSource listSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataControlBase"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        protected DataControlBase()
        {
            this.listSource = this.CreateListSource();
            this.listSource.CollectionChanged += this.OnListSourceCollectionChanged;
        }

        /// <summary>
        /// Gets or sets a collection used to generate the content of the <see cref="RadVirtualizingDataControl"/>. 
        /// </summary>
        /// <value>The object that is used to generate the content of the <see cref="RadVirtualizingDataControl"/>. The default is null.</value>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.GetValue(ItemsSourceProperty) as IEnumerable;
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        RadListSource IListSourceProvider.ListSource
        {
            get
            {
                return this.listSource;
            }
        }

        internal RadListSource ListSource
        {
            get
            {
                return this.listSource;
            }
        }

        internal virtual RadListSource CreateListSource()
        {
            return new RadListSource();
        }

        /// <summary>
        /// Called before the core ItemsSource changed logic is executed.
        /// </summary>
        internal virtual void OnBeforeItemsSourceChanged(IEnumerable oldSource)
        {
        }

        /// <summary>
        /// Called after the core ItemsSource changed logic has been executed.
        /// </summary>
        internal virtual void OnAfterItemsSourceChanged(IEnumerable oldSource)
        {
            if (this.listSource != null)
            {
                this.listSource.SourceCollection = this.ItemsSource;
            }
        }

        /// <summary>
        /// Occurs when the underlying source collection has changed (valid when the collection implements INotifyCollectionChanged).
        /// </summary>
        protected virtual void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the <see cref="ItemsSource"/> property has changed.
        /// </summary>
        protected virtual void OnItemsSourceChanged(IEnumerable oldSource)
        {
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataControlBase control = sender as DataControlBase;

            control.OnBeforeItemsSourceChanged(e.OldValue as IEnumerable);
            control.OnItemsSourceChanged(e.OldValue as IEnumerable);
            control.OnAfterItemsSourceChanged(e.OldValue as IEnumerable);
        }

        private void OnListSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnItemsChanged(e);
            this.InvalidateUI();
        }
    }
}
