using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Telerik.Core
{
    internal class WeakEventHandlerList<TArgs> where TArgs : class
    {
        private KnownEvents knownEvent;
        private WeakReference<IWeakEventListener> eventListener;

        public WeakEventHandlerList(IWeakEventListener listener, KnownEvents knownEvent)
        {
            this.eventListener = new WeakReference<IWeakEventListener>(listener);
            this.knownEvent = knownEvent;
        }

        public void Unsubscribe(object sender)
        {
            this.Update(sender, false);
        }

        public void Subscribe(object sender)
        {
            this.Update(sender, true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "There is only one cast, which is cheaper than a cast plus a local variable declaration.")]
        private void Update(object sender, bool subscribe)
        {
            switch (this.knownEvent)
            {
                case KnownEvents.CollectionChanged:
                    if (subscribe)
                    {
                        (sender as INotifyCollectionChanged).CollectionChanged += this.OnCollectionChanged;
                    }
                    else
                    {
                        (sender as INotifyCollectionChanged).CollectionChanged -= this.OnCollectionChanged;
                    }

                    break;
                case KnownEvents.PropertyChanged:
                    if (subscribe)
                    {
                        (sender as INotifyPropertyChanged).PropertyChanged += this.OnPropertyChanged;
                    }
                    else
                    {
                        (sender as INotifyPropertyChanged).PropertyChanged -= this.OnPropertyChanged;
                    }

                    break;
                case KnownEvents.VectorChanged:
                    if (subscribe)
                    {
                        (sender as ICollectionView).VectorChanged += this.OnVectorChanged;
                    }
                    else
                    {
                        (sender as ICollectionView).VectorChanged -= this.OnVectorChanged;
                    }

                    break;
            }
        }

        private void ProcessEvent(object sender, object args)
        {
            IWeakEventListener listener;
            if (this.eventListener.TryGetTarget(out listener))
            {
                listener.ReceiveEvent(sender, args);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ProcessEvent(sender, e);
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            this.ProcessEvent(sender, e);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ProcessEvent(sender, e);
        }

        private void OnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            this.ProcessEvent(sender, e);
        }
    }
}