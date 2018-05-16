using System.ComponentModel;

namespace Telerik.Core.Data
{
    internal interface IPropertyChangedListener
    {
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}