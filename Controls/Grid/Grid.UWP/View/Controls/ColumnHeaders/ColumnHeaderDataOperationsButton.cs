using Telerik.UI.Xaml.Controls.Primitives.Common;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a ColumnHeaderDataOperationsButton control.
    /// </summary>
    public class ColumnHeaderDataOperationsButton : InlineButton
    {
        /// <summary>
        /// Identifies the <see cref="IconText"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IconTextProperty =
            DependencyProperty.Register(nameof(IconText), typeof(string), typeof(ColumnHeaderDataOperationsButton), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnHeaderDataOperationsButton"/> class.
        /// </summary>
        public ColumnHeaderDataOperationsButton()
        {
            this.DefaultStyleKey = typeof(ColumnHeaderDataOperationsButton);
        }

        /// <summary>
        /// Gets or sets the text of the Button's Icon.
        /// </summary>
        public string IconText
        {
            get { return (string)GetValue(IconTextProperty); }
            set { this.SetValue(IconTextProperty, value); }
        }
    }
}
