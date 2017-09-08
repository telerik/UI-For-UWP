using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Primitives.RangeSlider
{
    internal class ToolTipContext : INotifyPropertyChanged
    {
        private string startValue;
        private string endValue;
        private string range;

        /// <summary>
        /// Raised whenever a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public string StartValue
        {
            get
            {
                return this.startValue;
            }

            set
            {
                this.startValue = value;
                this.OnPropertyChanged(nameof(this.StartValue));
            }
        }

        public string EndValue
        {
            get
            {
                return this.endValue;
            }

            set
            {
                this.endValue = value;
                this.OnPropertyChanged(nameof(this.EndValue));
            }
        }

        public string Range
        {
            get
            {
                return this.range;
            }

            set
            {
                this.range = value;
                this.OnPropertyChanged(nameof(this.Range));
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propName">Name of the prop.</param>
        protected void OnPropertyChanged(string propName)
        {
            PropertyChangedEventHandler eh = this.PropertyChanged;
            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
