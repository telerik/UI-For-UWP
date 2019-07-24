using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Telerik.Core.Data;
using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a class that enables a user to set specific properties for the month view.
    /// </summary>
    public class MonthViewSettings : RadDependencyObject, ICollectionChangedListener, IPropertyChangedListener
    {
        /// <summary>
        /// Identifies the SpecialSlotsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty SpecialSlotsSourceProperty =
            DependencyProperty.Register(nameof(SpecialSlotsSource), typeof(IEnumerable<Slot>), typeof(MonthViewSettings), new PropertyMetadata(null, OnSpecialSlotsSourcePropertyChanged));

        private WeakCollectionChangedListener specialSlotsCollectionChangedListener;
        private List<WeakPropertyChangedListener> specialSlotsPropertyChangedListeners = new List<WeakPropertyChangedListener>();

        /// <summary>
        /// Gets or sets the special slots source.
        /// </summary>
        /// <value>The special slots source.</value>
        public IEnumerable<Slot> SpecialSlotsSource
        {
            get
            {
                return (IEnumerable<Slot>)this.GetValue(SpecialSlotsSourceProperty);
            }
            set
            {
                this.SetValue(SpecialSlotsSourceProperty, value);
            }
        }

        private static void OnSpecialSlotsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            MonthViewSettings settings = (MonthViewSettings)sender;
            INotifyCollectionChanged oldSlotsSource = args.OldValue as INotifyCollectionChanged;
            if (oldSlotsSource != null)
            {
                var listener = settings.specialSlotsCollectionChangedListener;
                if (listener != null)
                {
                    listener.Detach();
                    listener = null;
                }

                int count = settings.specialSlotsPropertyChangedListeners != null ? settings.specialSlotsPropertyChangedListeners.Count : 0;
                while (count > 0)
                {
                    var propertyListener = settings.specialSlotsPropertyChangedListeners[0];
                    settings.specialSlotsPropertyChangedListeners.RemoveAt(0);
                    propertyListener.Detach();
                    propertyListener = null;
                    count--;
                }
            }

            INotifyCollectionChanged newSlotsSource = args.NewValue as INotifyCollectionChanged;
            if (newSlotsSource != null)
            {
                settings.specialSlotsCollectionChangedListener = WeakCollectionChangedListener.CreateIfNecessary(newSlotsSource, settings);

                foreach (Slot slot in (IEnumerable<Slot>)newSlotsSource)
                {
                    var listener = WeakPropertyChangedListener.CreateIfNecessary(slot, settings);
                    if (listener != null)
                    {
                        settings.specialSlotsPropertyChangedListeners.Add(listener);
                    }
                }
            }
        }

        /// <summary>
        /// Implementation of the <see cref="ICollectionChangedListener" /> interface.
        /// </summary>
        /// <param name="sender">The collection sending the event.</param>
        /// <param name="e">The event args.</param>
        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Slot slot in e.NewItems)
                    {
                        WeakPropertyChangedListener newListener = WeakPropertyChangedListener.CreateIfNecessary(slot, this);
                        if (newListener != null)
                        {
                            this.specialSlotsPropertyChangedListeners.Add(newListener);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (this.specialSlotsPropertyChangedListeners != null && this.specialSlotsPropertyChangedListeners.Count > 0)
                    {
                        foreach (Slot slot in e.OldItems)
                        {
                            WeakPropertyChangedListener oldPropertyListener = this.specialSlotsPropertyChangedListeners[e.OldStartingIndex];
                            if (oldPropertyListener != null)
                            {
                                this.specialSlotsPropertyChangedListeners.Remove(oldPropertyListener);
                                oldPropertyListener.Detach();
                                oldPropertyListener = null;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    WeakPropertyChangedListener propertyListener = this.specialSlotsPropertyChangedListeners[e.OldStartingIndex];
                    if (propertyListener != null)
                    {
                        this.specialSlotsPropertyChangedListeners.Remove(propertyListener);
                        propertyListener.Detach();
                        propertyListener = null;
                    }

                    WeakPropertyChangedListener listener = WeakPropertyChangedListener.CreateIfNecessary(e.NewItems[0], this);
                    if (listener != null)
                    {
                        this.specialSlotsPropertyChangedListeners.Add(listener);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Implementation of the <see cref="IPropertyChangedListener" /> interface.
        /// </summary>
        /// <param name="sender">The sender of the property changed.</param>
        /// <param name="e">The arguments of the event.</param>
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO: Update when property changes
        }
    }
}
