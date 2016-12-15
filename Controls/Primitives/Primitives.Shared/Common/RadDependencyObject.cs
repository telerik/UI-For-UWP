using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents an extended <see cref="DependencyObject"/> that recognizes whether a property is changed internally within the class or programmatically by the user.
    /// </summary>
    [Bindable]
    public abstract class RadDependencyObject : DependencyObject
    {
        private byte internalPropertyChange;

        internal bool IsInternalPropertyChange
        {
            get
            {
                return this.internalPropertyChange > 0;
            }
        }

        internal void ChangePropertyInternally(DependencyProperty property, object value)
        {
            this.internalPropertyChange++;
            this.SetValue(property, value);
            this.internalPropertyChange--;
        }
    }
}
