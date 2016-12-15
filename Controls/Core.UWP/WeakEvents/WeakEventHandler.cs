using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Telerik.Core.Data;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace Telerik.Core
{
    internal class WeakEventHandler<TArgs> where TArgs : class
    {
        private WeakReference eventListener;
        private WeakReference eventSender;
        private KnownEvents knownEvent;

        public WeakEventHandler(object sender, IWeakEventListener listener, KnownEvents knownEvent)
        {
            this.eventListener = new WeakReference(listener);
            this.eventSender = new WeakReference(sender);
            this.knownEvent = knownEvent;

            this.Update(sender, true);
        }

        public void Unsubscribe()
        {
            if (this.eventSender == null || !this.eventSender.IsAlive)
            {
                return;
            }

            this.Update(this.eventSender.Target, false);
            this.eventSender = null;
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
                case KnownEvents.CurrentItemChanged:
                    if (subscribe)
                    {
                        (sender as ICollectionView).CurrentChanged += this.OnCurrentChanged;
                    }
                    else
                    {
                        (sender as ICollectionView).CurrentChanged -= this.OnCurrentChanged;
                    }

                    break;
                case KnownEvents.CanExecuteChanged:
                    if (subscribe)
                    {
                        (sender as ICommand).CanExecuteChanged += this.OnCanExecuteChanged;
                    }
                    else
                    {
                        (sender as ICommand).CanExecuteChanged -= this.OnCanExecuteChanged;
                    }

                    break;
            }
        }

        private void OnEvent(object sender, TArgs e)
        {
            this.ProcessEvent(sender, e);
        }

        private void ProcessEvent(object sender, object args)
        {
            if (this.eventListener.IsAlive)
            {
                (this.eventListener.Target as IWeakEventListener).ReceiveEvent(sender, args);
            }
            else
            {
                this.Unsubscribe();
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ProcessEvent(sender, e);
        }

        private void OnCurrentChanged(object sender, object e)
        {
            this.ProcessEvent(sender, new NotifyCurrentItemChangedEventArgs());
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ProcessEvent(sender, e);
        }

        private void OnVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            this.ProcessEvent(sender, e);
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            this.ProcessEvent(sender, e);
        }
    }
}