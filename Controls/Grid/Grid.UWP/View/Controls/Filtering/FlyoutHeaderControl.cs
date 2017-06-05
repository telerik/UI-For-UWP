using System.Windows.Input;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a FlyoutHeaderControl control.
    /// </summary>
    public class FlyoutHeaderControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Icon"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(object), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IconStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IconStyleProperty =
            DependencyProperty.Register(nameof(IconStyle), typeof(Style), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ContentStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ContentStyleProperty =
            DependencyProperty.Register(nameof(ContentStyle), typeof(Style), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ButtonStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty =
            DependencyProperty.Register(nameof(ButtonStyle), typeof(Style), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(FlyoutHeaderControl), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutHeaderControl"/> class.
        /// </summary>
        public FlyoutHeaderControl()
        {
            this.DefaultStyleKey = typeof(FlyoutHeaderControl);
        }

        /// <summary>
        /// Gets or sets the FlyoutHeader's Content.
        /// </summary>
        public object Content
        {
            get
            {
                return this.GetValue(ContentProperty);
            }
            set
            {
                this.SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the FlyoutHeader's icon.
        /// </summary>
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FlyoutHeader's Icon style.
        /// </summary>
        public Style IconStyle
        {
            get { return (Style)GetValue(IconStyleProperty); }
            set { this.SetValue(IconStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FlyoutHeader's Content style.
        /// </summary>
        public Style ContentStyle
        {
            get { return (Style)GetValue(ContentStyleProperty); }
            set { this.SetValue(ContentStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FlyoutHeader's Button style.
        /// </summary>
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { this.SetValue(ButtonStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FlyoutHeader's Command.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets the Owner of the <see cref="FlyoutHeaderControl"/>.
        /// </summary>
        public DataGridFilteringFlyout Owner { get; internal set; }
    }
}