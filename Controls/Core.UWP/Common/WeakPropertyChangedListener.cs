using System;
using System.ComponentModel;

namespace Telerik.Core.Data
{
    internal class WeakPropertyChangedListener
    {
        private INotifyPropertyChanged source;
        private WeakReference weakListener;

        private WeakPropertyChangedListener(INotifyPropertyChanged source, IPropertyChangedListener listener)
        {
            this.source = source;
            this.source.PropertyChanged += new PropertyChangedEventHandler(this.SourcePropertyChanged);
            this.weakListener = new WeakReference(listener);
        }

        internal static WeakPropertyChangedListener CreateIfNecessary(object source, IPropertyChangedListener listener)
        {
            var inpc = source as INotifyPropertyChanged;
            if (inpc != null)
            {
                return new WeakPropertyChangedListener(inpc, listener);
            }
            return null;
        }

        internal void Detach()
        {
            if (this.source != null)
            {
                this.source.PropertyChanged -= new PropertyChangedEventHandler(this.SourcePropertyChanged);
                this.source = null;
                this.weakListener = null;
            }
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.weakListener != null)
            {
                IPropertyChangedListener target = this.weakListener.Target as IPropertyChangedListener;
                if (target != null)
                {
                    target.OnPropertyChanged(sender, e);
                }
                else
                {
                    this.Detach();
                }
            }
        }
    }
}
