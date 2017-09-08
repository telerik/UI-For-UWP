using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Chart.Tests
{
    public class BusinessObject : INotifyPropertyChanged
    {
        private double? value;
        private double secondValue = 0d;
        private int intValue;
        private object category;
        private bool visibleInLegend;

        public BusinessObject()
        {
        }

        public BusinessObject(object category, double? value)
        {
            this.category = category;
            this.value = value;
            this.visibleInLegend = true;
        }

        public int IntValue
        {
            get
            {
                return this.intValue;
            }
            set
            {
                this.intValue = value;
                this.OnPropertyChanged("IntValue");
            }
        }

        public double? Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.OnPropertyChanged("Value");
            }
        }

        public double SecondValue
        {
            get
            {
                return this.secondValue;
            }
            set
            {
                this.secondValue = value;
                this.OnPropertyChanged("SecondValue");
            }
        }

        public object Category
        {
            get
            {
                return this.category;
            }
            set
            {
                this.category = value;
                this.OnPropertyChanged("Category");
            }
        }

        public bool VisibleInLegend
        {
            get
            {
                return this.visibleInLegend;
            }
            set
            {
                this.visibleInLegend = value;
                this.OnPropertyChanged("VisibleInLegend");
            }
        }

        private void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
