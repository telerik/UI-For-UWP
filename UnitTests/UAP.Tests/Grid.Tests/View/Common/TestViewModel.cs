using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public class TestViewModel : ViewModelBase
    {
        private string name;
        private string city;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                this.city = value;
                this.OnPropertyChanged();
            }
        }
    }
}
