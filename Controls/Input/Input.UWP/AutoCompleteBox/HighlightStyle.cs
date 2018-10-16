using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Defines the style settings used to highlight portion of a TextBlock within a suggestion item.
    /// </summary>
    [Bindable]
    public class HighlightStyle : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="FontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.Register(nameof(FontStyle), typeof(FontStyle), typeof(HighlightStyle), null);

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register(nameof(FontWeight), typeof(FontWeightName), typeof(HighlightStyle), null);

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(HighlightStyle), null);

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(HighlightStyle), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x26, 0xA0, 0xDA))));

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(HighlightStyle), null);

        /// <summary>
        /// Gets or sets an instance of the <see cref="FontFamily"/> class
        /// that defines the family of the font used to highlight text portion
        /// within a suggestion item.
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                return this.GetValue(FontFamilyProperty) as FontFamily;
            }
            set
            {
                this.SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="FontWeight"/> struct
        /// that defines the weight of the font used to highlight text portion 
        /// within a suggestion item.
        /// </summary>
        public FontWeightName FontWeight
        {
            get
            {
                return (FontWeightName)this.GetValue(FontWeightProperty);
            }
            set
            {
                this.SetValue(FontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="FontStyle"/> struct
        /// that defines the style of the font used to highlight text portion
        /// within a suggestion item.
        /// </summary>
        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)this.GetValue(FontStyleProperty);
            }
            set
            {
                this.SetValue(FontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="Brush"/> class
        /// that defines the color used to highlight text portion
        /// within a suggestion item.
        /// </summary>
        public Brush Foreground
        {
            get
            {
                return this.GetValue(ForegroundProperty) as Brush;
            }
            set
            {
                this.SetValue(ForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value
        /// that defines the font size used to highlight text portion
        /// within a suggestion item.
        /// </summary>
        public double FontSize
        {
            get
            {
                return (double)this.GetValue(FontSizeProperty);
            }
            set
            {
                this.SetValue(FontSizeProperty, value);
            }
        }
    }
}
