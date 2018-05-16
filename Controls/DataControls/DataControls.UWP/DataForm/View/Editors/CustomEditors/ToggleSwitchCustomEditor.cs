using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a ToggleSwitchCustomEditor control.
    /// </summary>
    public class ToggleSwitchCustomEditor : CustomEditorBase<ToggleSwitch>
    {
        /// <summary>
        /// Identifies the <see cref="SelectedBackgroundBrush"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundBrushProperty =
            DependencyProperty.Register(nameof(SelectedBackgroundBrush), typeof(Brush), typeof(ToggleSwitchCustomEditor), new PropertyMetadata(null, OnSelectedBackgroundBrushChanged));
        
        /// <summary>
        /// Identifies the <see cref="OffStateBackground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty PointerOverBackgroundBrushProperty =
            DependencyProperty.Register(nameof(PointerOverBackgroundBrush), typeof(Brush), typeof(ToggleSwitchCustomEditor), new PropertyMetadata(null, OnPointerOverBackgroundBrushPropertyChanged));
        
        /// <summary>
        /// Identifies the <see cref="OffStateBackground"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty OffStateBackgroundProperty =
            DependencyProperty.Register(nameof(OffStateBackground), typeof(Brush), typeof(ToggleSwitchCustomEditor), new PropertyMetadata(null, OnOffStateBackgroundPropertyChanged));
        
        private const string SwitchKnobBoundsPartName = "SwitchKnobBounds";
        private const string SwitchKnobOffPartName = "SwitchKnobOff";
        private const string OuterBorderPartName = "OuterBorder";
        private Rectangle switchKnobRect;
        private Ellipse switchKnobOffEllipse;
        private Rectangle outerBorderRect;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitchCustomEditor"/> class.
        /// </summary>
        public ToggleSwitchCustomEditor()
        {
            this.DefaultStyleKey = typeof(ToggleSwitchCustomEditor);
        }

        /// <summary>
        /// Gets or sets the Background of the rectangle of the ToggleSwitch knob.
        /// </summary>
        public Brush SelectedBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(SelectedBackgroundBrushProperty);
            }
            set
            {
                this.SetValue(SelectedBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Background of the ellipse of the ToggleSwitch knob.
        /// </summary>
        public Brush OffStateBackground
        {
            get
            {
                return (Brush)GetValue(OffStateBackgroundProperty);
            }
            set
            {
                this.SetValue(OffStateBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Background of the ellipse of the ToggleSwitch knob.
        /// </summary>
        public Brush PointerOverBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(PointerOverBackgroundBrushProperty);
            }
            set
            {
                this.SetValue(PointerOverBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.EditorControl != null)
            {
                this.EditorControl.Loaded += this.OnToggleSwitchEditorLoaded;
            }
        }

        /// <inheritdoc />
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);

            if (this.switchKnobRect != null)
            {
                this.switchKnobRect.Fill = this.PointerOverBackgroundBrush;
                this.switchKnobRect.Stroke = this.PointerOverBackgroundBrush;
            }
        }

        /// <inheritdoc />
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);

            if (this.switchKnobRect != null)
            {
                this.switchKnobRect.Fill = this.SelectedBackgroundBrush;
                this.switchKnobRect.Stroke = this.SelectedBackgroundBrush;
            }
        }

        private static void OnSelectedBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitchEditor = d as ToggleSwitchCustomEditor;
            toggleSwitchEditor.UpdateSwitchBrushes();
        }

        private static void OnOffStateBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitchEditor = d as ToggleSwitchCustomEditor;
            toggleSwitchEditor.UpdateSwitchBrushes();
        }

        private static void OnPointerOverBackgroundBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleSwitchEditor = d as ToggleSwitchCustomEditor;
            toggleSwitchEditor.UpdateSwitchBrushes();
        }

        private void OnToggleSwitchEditorLoaded(object sender, RoutedEventArgs e)
        {
            this.switchKnobRect = ElementTreeHelper.FindVisualDescendant<Rectangle>(this.EditorControl, a => a.GetType() == typeof(Rectangle) && ((Rectangle)a).Name == SwitchKnobBoundsPartName);
            this.switchKnobOffEllipse = ElementTreeHelper.FindVisualDescendant<Ellipse>(this.EditorControl, a => a.GetType() == typeof(Ellipse) && ((Ellipse)a).Name == SwitchKnobOffPartName);
            this.outerBorderRect = ElementTreeHelper.FindVisualDescendant<Rectangle>(this.EditorControl, a => a.GetType() == typeof(Rectangle) && ((Rectangle)a).Name == OuterBorderPartName);
            
            this.UpdateSwitchBrushes();
            this.EditorControl.Loaded -= this.OnToggleSwitchEditorLoaded;
        }
        
        private void UpdateSwitchBrushes()
        {
            if (this.switchKnobRect != null)
            {
                this.switchKnobRect.Fill = this.SelectedBackgroundBrush;
            }

            if (this.switchKnobOffEllipse != null)
            {
                this.switchKnobOffEllipse.Fill = this.OffStateBackground;
                this.outerBorderRect.Stroke = this.OffStateBackground;
            }
        }
    }
}
