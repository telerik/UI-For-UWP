using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Core;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Data representation of the legend items displayed in the <see cref="RadLegendControl"/>.
    /// </summary>
    public class LegendItem : ViewModelBase
    {
        private string title;
        private Brush fill;
        private Brush stroke;

        /// <summary>
        /// Gets or sets the Legend Title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Legend Fill.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return this.fill;
            }
            set
            {
                this.fill = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Legend Stroke.
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return this.stroke;
            }
            set
            {
                this.stroke = value;
                this.OnPropertyChanged();
            }
        }
    }
}
