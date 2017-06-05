using System;
using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// A special data item that may be visualized in a <see cref="RadLoopingList"/> instance.
    /// </summary>
    internal class LoopingListDataItem : INotifyPropertyChanged
    {
        private string text;
        private object item;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopingListDataItem"/> class.
        /// </summary>
        public LoopingListDataItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopingListDataItem"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public LoopingListDataItem(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Raised whenever a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the text associated with this item.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.OnPropertyChanged(nameof(this.Text));
                }
            }
        }

        public object Item 
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
                this.OnPropertyChanged(nameof(this.Item));
            }
        }
        
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.text))
            {
                return this.text;
            }

            return base.ToString();
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
