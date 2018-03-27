using System;
using System.Collections.Specialized;

namespace Telerik.Core.Data
{
    internal class WeakCollectionChangedListener
    {
        private INotifyCollectionChanged source;
        private WeakReference weakListener;

        private WeakCollectionChangedListener(INotifyCollectionChanged source, ICollectionChangedListener listener)
        {
            this.source = source;
            this.source.CollectionChanged += new NotifyCollectionChangedEventHandler(this.SourceCollectionChanged);
            this.weakListener = new WeakReference(listener);
        }

        internal static WeakCollectionChangedListener CreateIfNecessary(object source, ICollectionChangedListener listener)
        {
            var incc = source as INotifyCollectionChanged;
            if (incc != null)
            {
                return new WeakCollectionChangedListener(incc, listener);
            }
            return null;
        }

        internal void Detach()
        {
            if (this.source != null)
            {
                this.source.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.SourceCollectionChanged);
                this.source = null;
                this.weakListener = null;
            }
        }

        private void SourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.weakListener != null)
            {
                ICollectionChangedListener target = this.weakListener.Target as ICollectionChangedListener;
                if (target != null)
                {
                    target.OnCollectionChanged(sender, e);
                }
                else
                {
                    this.Detach();
                }
            }
        }
    }
}