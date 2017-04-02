using System;
using System.ComponentModel;
using System.Globalization;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input.NumericBox;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents a special <see cref="RangeControlBase"/> implementation that allows for numeric input. The control consists of a TextBox part which may be edited with numerical value directly and two buttons to increase/decrease the current value.
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(NumericTextBox))]
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(Button))]
    [TemplateVisualState(Name = "WatermarkHidden", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "WatermarkVisible", GroupName = "CommonStates")]
    public class RadNumericBox : RangeInputBase, ICultureAware
    {
        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InputScopeProperty = DependencyProperty.Register(nameof(InputScope), typeof(InputScopeNameValue), typeof(RadNumericBox), new PropertyMetadata(InputScopeNameValue.Number, OnScopeChanged));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Object), typeof(RadNumericBox), new PropertyMetadata(null, OnValueChanged)); // TODO: double? is not supported as PropertyType, check with next WinRT versions

        /// <summary>
        /// Identifies the <see cref="AllowNullValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowNullValueProperty =
            DependencyProperty.Register(nameof(AllowNullValue), typeof(bool), typeof(RadNumericBox), new PropertyMetadata(true, OnAllowNullValueChanged));

        /// <summary>
        /// Identifies the <see cref="ValueFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register(nameof(ValueFormat), typeof(string), typeof(RadNumericBox), new PropertyMetadata("{0,0:N2}", OnValueFormatChanged));

        /// <summary>
        /// Identifies the <see cref="ValueString"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueStringProperty =
            DependencyProperty.Register(nameof(ValueString), typeof(string), typeof(RadNumericBox), new PropertyMetadata(null, OnValueStringChanged));

        /// <summary>
        /// Identifies the <see cref="IncreaseButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IncreaseButtonStyleProperty =
            DependencyProperty.Register(nameof(IncreaseButtonStyle), typeof(Style), typeof(RadNumericBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DecreaseButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecreaseButtonStyleProperty =
            DependencyProperty.Register(nameof(DecreaseButtonStyle), typeof(Style), typeof(RadNumericBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Watermark"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(nameof(Watermark), typeof(object), typeof(RadNumericBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="WatermarkTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register(nameof(WatermarkTemplate), typeof(object), typeof(RadNumericBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ButtonsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonsVisibilityProperty =
            DependencyProperty.Register(nameof(ButtonsVisibility), typeof(Visibility), typeof(RadNumericBox), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="IsEditable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(RadNumericBox), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="AcceptsDecimalSeparator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AcceptsDecimalSeparatorProperty =
            DependencyProperty.Register(nameof(AcceptsDecimalSeparator), typeof(bool), typeof(RadNumericBox), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="DecimalSeparatorKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DecimalSeparatorKeyProperty =
            DependencyProperty.Register(nameof(DecimalSeparatorKey), typeof(int), typeof(RadNumericBox), new PropertyMetadata(-1));

        /// <summary>
        /// Identifies the <see cref="ValueFormatSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueFormatSelectorProperty =
            DependencyProperty.Register(nameof(ValueFormatSelector), typeof(IFormatSelector), typeof(RadNumericBox), new PropertyMetadata(null, OnValueFormatSelectorChanged));

        private const int CommaKey = 188;
        private const int DashKey = 189;
        private const int DotKey = 190;

        private CultureInfo currentCulture = CultureInfo.CurrentCulture;
        private TextBox textBox;
        private Button increaseButton;
        private Button decreaseButton;
        private KeyEventHandler textBoxKeyDownHandler;
        private bool isNegative;
        private bool isDecimal;
        private bool isEditing;

        private bool updatingValue;
        private bool allowNullValueCache;
        private double? valueCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadNumericBox" /> class.
        /// </summary>
        public RadNumericBox()
        {
            this.DefaultStyleKey = typeof(RadNumericBox);

            this.textBoxKeyDownHandler = new KeyEventHandler(this.OnTextBoxKeyDown);

            this.allowNullValueCache = true;
        }

        /// <summary>
        /// Occurs when the current value has changed.
        /// </summary>
        public event EventHandler ValueChanged;
        
        /// <summary>
        /// Gets or sets the context for input used by this RadNumericBox.
        /// </summary>
        public InputScopeNameValue InputScope
        {
            get { return (InputScopeNameValue)this.GetValue(InputScopeProperty); }
            set { this.SetValue(InputScopeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Value"/> property may be set to null.
        /// </summary>
        public bool AllowNullValue
        {
            get
            {
                return this.allowNullValueCache;
            }
            set
            {
                this.SetValue(AllowNullValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the currently displayed value. The value may be coerced to fit the current range restrictions.
        /// </summary>
        /// <value>
        ///  The default value is 0.
        /// </value>
        public double? Value
        {
            get
            {
                return this.valueCache;
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an additional integer VirtualKey which will be treated as a decimal separator(dot key or comma) depending on the current culture. 
        /// This property is introduced to allow users to handle decimal separator for some Keyboard Languages like Russian, Ukrainian and etc. which may have 
        /// different integer mapping.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int DecimalSeparatorKey
        {
            get { return (int)GetValue(DecimalSeparatorKeyProperty); }
            set { SetValue(DecimalSeparatorKeyProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBox part of the component may be directly edited through the keyboard.
        /// </summary>
        public bool AcceptsDecimalSeparator
        {
            get
            {
                return (bool)this.GetValue(AcceptsDecimalSeparatorProperty);
            }
            set
            {
                this.SetValue(AcceptsDecimalSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBox part of the component can be directly edited through the keyboard. The default value is <c>true</c>.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        ///  &lt;telerikInput:RadNumericBox x:Name="numericBox" IsEditable="False"/&gt;
        /// </code>
        /// C#
        /// <code language="c#"> 
        /// this.numericBox.IsEditable = false;
        /// </code>
        /// </example>
        public bool IsEditable
        {
            get
            {
                return (bool)this.GetValue(IsEditableProperty);
            }
            set
            {
                this.SetValue(IsEditableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Visibility"/> value for the Increase and Decrease buttons. The default value is <c>Visible</c>.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadNumericBox x:Name="numericBox" ButtonsVisibility="Collapsed"/&gt;
        /// </code>
        /// <code language="c#">
        /// this.numericBox.ButtonsVisibility = Visibility.Collapsed;
        /// </code>
        /// </example>
        public Visibility ButtonsVisibility
        {
            get
            {
                return (Visibility)this.GetValue(ButtonsVisibilityProperty);
            }
            set
            {
                this.SetValue(ButtonsVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the object that represents the Watermark displayed by the control. The Watermark is visible when the current Value is not set (null).
        /// </summary>
        /// <value>
        /// The default value is <c>null</c>
        /// </value>
        /// <example>
        /// <code language="xaml">
        ///  &lt;telerikInput:RadNumericBox x:Name="numericBox" Watermark="WaterMark"/&gt;
        /// </code>
        /// <code language="c#">
        ///  this.numericBox.Watermark = "WaterMark";
        /// </code>
        /// </example>
        public object Watermark
        {
            get
            {
                return this.GetValue(WatermarkProperty);
            }
            set
            {
                this.SetValue(WatermarkProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance that defines the appearance of the <see cref="Watermark"/> value.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        ///   &lt;telerikInput:RadNumericBox x:Name="numericBox"&gt;
        ///    &lt;telerikInput:RadNumericBox.WatermarkTemplate&gt;
        ///       &lt;DataTemplate&gt;
        ///            &lt;Ellipse Width="20" Height="20" Fill="Red" Stroke="Red"/&gt;
        ///        &lt;/DataTemplate&gt;
        ///     &lt;/telerikInput:RadNumericBox.WatermarkTemplate&gt;
        ///   &lt;/telerikInput:RadNumericBox&gt;
        /// </code>
        /// </example>
        public DataTemplate WatermarkTemplate
        {
            get
            {
                return this.GetValue(WatermarkTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(WatermarkTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> object that defines the appearance of the IncreaseButton control part.
        /// The TargetType of the Style object should point to the <see cref="Telerik.UI.Xaml.Controls.Primitives.Common.InlineButton"/> type.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        ///   &lt;telerikInput:RadNumericBox.IncreaseButtonStyle&gt;
        ///        &lt;Style TargetType="PrimitivesCommon:InlineButton"&gt;
        ///            &lt;Setter Property="Background" Value="Red"/&gt;
        ///            &lt;Setter Property="Width" Value="35"/&gt;
        ///            &lt;Setter Property="Height" Value="35"/&gt;
        ///        &lt;/Style&gt;
        ///    &lt;/telerikInput:RadNumericBox.IncreaseButtonStyle&gt;
        /// </code>
        /// </example>
        public Style IncreaseButtonStyle
        {
            get
            {
                return this.GetValue(IncreaseButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(IncreaseButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> object that defines the appearance of the DecreaseButton control part.
        /// The TargetType of the Style object should point to the <see cref="Telerik.UI.Xaml.Controls.Primitives.Common.InlineButton"/> type.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        ///     &lt;telerikInput:RadNumericBox.DecreaseButtonStyle&gt;
        ///        &lt;Style TargetType="PrimitivesCommon:InlineButton"&gt;
        ///            &lt;Setter Property="Background" Value="Red"/&gt;
        ///            &lt;Setter Property="Width" Value="35"/&gt;
        ///            &lt;Setter Property="Height" Value="35"/&gt;
        ///        &lt;/Style&gt;
        ///     &lt;/telerikInput:RadNumericBox.DecreaseButtonStyle&gt;
        /// </code>
        /// </example>
        public Style DecreaseButtonStyle
        {
            get
            {
                return this.GetValue(DecreaseButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DecreaseButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string that is used to format the current Value of the component. 
        /// The String.Format routine is used and the value specified should be in the form required by this method: { index[,alignment][ :formatString] }.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        ///   &lt;telerikInput:RadNumericBox x:Name="numericBox" ValueFormat="{}{0:C}" /&gt;
        /// </code>
        /// C#
        /// <code language="c#">
        /// this.numericBox.ValueFormat = "{0:C}";
        /// </code>
        /// </example>
        public string ValueFormat
        {
            get
            {
                return (string)this.GetValue(ValueFormatProperty);
            }
            set
            {
                this.SetValue(ValueFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the IFormatSelector that is used to determine the format for the current Value of the component. 
        /// The String.Format routine is used and the value specified should be in the form required by this method: { index[,alignment][ :formatString] }.
        /// </summary>
        public IFormatSelector ValueFormatSelector
        {
            get
            {
                return (IFormatSelector)GetValue(ValueFormatSelectorProperty);
            }
            set
            {
                SetValue(ValueFormatSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets the current string representation of the Value property, as displayed in the TextBox part.
        /// </summary>
        public string ValueString
        {
            get
            {
                return (string)this.GetValue(ValueStringProperty);
            }
        }

        CultureInfo ICultureAware.CurrentCulture
        {
            get
            {
                return this.currentCulture;
            }
        }

        /// <summary>
        /// Gets the button used to decrease the current value with the SmallChange. Exposed for testing purposes only.
        /// </summary>
        internal Button DecreaseButton
        {
            get
            {
                return this.decreaseButton;
            }
        }

        /// <summary>
        /// Gets the button used to increase the current value with the SmallChange. Exposed for testing purposes only.
        /// </summary>
        internal Button IncreaseButton
        {
            get
            {
                return this.increaseButton;
            }
        }

        /// <summary>
        /// Gets the TextBox part of the component. Exposed for testing purposes only.
        /// </summary>
        internal TextBox TextBox
        {
            get
            {
                return this.textBox;
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.RadNumericBoxAutomationPeer(this);
        }

        private static bool IsAzertyKeyboard
        {
            get
            {
                return Windows.Globalization.Language.CurrentInputMethodLanguageTag.StartsWith("fr-"); ;
            }
        }

        void ICultureAware.OnCultureChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            this.currentCulture = newValue;
            if (this.currentCulture == null)
            {
                // cannot have null as current culture - fallback to the CurrentCulture.
                this.currentCulture = CultureInfo.CurrentCulture;
            }
        }

        internal override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);

            this.CoerceValue(this.Value);

            RadNumericBoxAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadNumericBoxAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMaximumPropertyChangedEvent((double)oldMaximum, (double)newMaximum);
            }
        }

        internal override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);

            this.CoerceValue(this.Value);

            RadNumericBoxAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadNumericBoxAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMaximumPropertyChangedEvent((double)oldMinimum, (double)newMinimum);
            }
        }

        /// <summary>
        /// Exposed for testing purposes only.
        /// </summary>
        internal void BeginEdit()
        {
            this.isEditing = true;

            this.UpdateVisualState(true);

            var value = this.Value;
            if (value.HasValue)
            {
                this.SetText(value.Value.ToString(this.currentCulture));
            }

            this.textBox.SelectAll();
        }

        /// <summary>
        /// Exposed for testing purposes only.
        /// </summary>
        internal void CommitEdit()
        {
            if (!this.isEditing)
            {
                return;
            }

            this.InvokeAsync(() =>
            {
                this.UpdateVisualState(true);
            });

            this.Value = this.TryParseValue();

            this.isEditing = false;

            this.UpdateTextBoxText();
        }

        /// <summary>
        /// Exposed for testing purposes only.
        /// </summary>
        /// <returns>True if the key is a valid character in the numeric context.</returns>
        internal bool PreviewKeyDown(VirtualKey key)
        {
            if (KeyboardHelper.IsModifierKeyDown(VirtualKey.Control))
            {
                // allow CTRL+[xxx] based operations
                return true;
            }

            if (IsNumericKey(key))
            {
                if (KeyboardHelper.IsModifierKeyDown(VirtualKey.Shift) && (key >= VirtualKey.NumberPad0 && key <= VirtualKey.NumberPad9))
                {
                    // handle SHIFT + Number (for example : with AZERTY keyboard layout : SHIFT + Number = Number).
                    // do not handle input when the SHIFT key is down/locked.
                    return false;
                }

                // allow base handling of numeric-related chars
                return true;
            }

            if (IsNegativeSign(key))
            {
                this.isNegative = true;
                this.ValidateText();
            }
            else if (this.IsDecimalSeparator(key))
            {
                this.isDecimal = this.AcceptsDecimalSeparator;

                if (this.isDecimal)
                {
                    var selectionStartCache = this.textBox.SelectionStart;

                    this.textBox.Text = this.textBox.Text.Remove(this.textBox.SelectionStart, this.textBox.SelectionLength);

                    this.textBox.SelectionStart = selectionStartCache;

                    if (this.textBox.Text == string.Empty)
                    {
                        this.SetText("0");
                    }
                    else if (this.textBox.Text == "-")
                    {
                        this.SetText("-0");
                    }
                }

                this.ValidateText();
            }
            else
            {
                switch (key)
                {
                    // Allowed key strokes
                    case VirtualKey.Left:
                    case VirtualKey.Right:
                    case VirtualKey.Back:
                    case VirtualKey.Delete:
                    case VirtualKey.Home:
                    case VirtualKey.End:
                    case VirtualKey.Tab:
                        return true;
                    case VirtualKey.Enter:
                        this.KillTextBoxFocus();
                        break;
                    case VirtualKey.Down:
                        this.DecrementValue(this.SmallChange);
                        break;
                    case VirtualKey.Up:
                        this.IncrementValue(this.SmallChange);
                        break;
                    case VirtualKey.PageDown:
                        this.DecrementValue(this.LargeChange);
                        break;
                    case VirtualKey.PageUp:
                        this.IncrementValue(this.LargeChange);
                        break;
                }

                if (key == VirtualKey.Down || key == VirtualKey.Up
                    || key == VirtualKey.PageDown || key == VirtualKey.PageUp)
                {
                    this.UpdateTextBoxText();
                }
            }

            return false;
        }

        /// <summary>
        /// Occurs when the <see cref="P:Value"/> property has changed. Raises the <see cref="E:ValueChanged"/> event.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            var eh = this.ValueChanged;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.textBox = this.GetTemplatePartField<TextBox>("PART_TextBox");
            applied = applied && this.textBox != null;

            this.increaseButton = this.GetTemplatePartField<Button>("PART_IncreaseButton");
            applied = applied && this.increaseButton != null;

            this.decreaseButton = this.GetTemplatePartField<Button>("PART_DecreaseButton");
            applied = applied && this.decreaseButton != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.CoerceValue(this.Value);

            this.UpdateInputScope(this.InputScope);
            this.textBox.AddHandler(TextBox.KeyDownEvent, this.textBoxKeyDownHandler, true);
            this.textBox.TextChanged += this.OnTextBoxTextChanged;
            this.textBox.GotFocus += this.OnTextBoxGotFocus;
            this.textBox.LostFocus += this.OnTextBoxLostFocus;

            this.increaseButton.Click += this.OnIncreaseButtonClick;
            this.decreaseButton.Click += this.OnDecreaseButtonClick;

            this.UpdateTextBoxText();
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.textBox.RemoveHandler(TextBox.KeyDownEvent, this.textBoxKeyDownHandler);
            this.textBox.TextChanged -= this.OnTextBoxTextChanged;
            this.textBox.GotFocus -= this.OnTextBoxGotFocus;
            this.textBox.LostFocus -= this.OnTextBoxLostFocus;

            this.increaseButton.Click -= this.OnIncreaseButtonClick;
            this.decreaseButton.Click -= this.OnDecreaseButtonClick;
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            
            this.IsTabStop = false;
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.NeedsWatermark())
            {
                return "WatermarkVisible";
            }

            return "WatermarkHidden";
        }

        /// <summary>
        /// Called before the PointerWheelChanged event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);

            if (e == null)
            {
                return;
            }

            int delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
            if (delta < 0)
            {
                this.DecrementValue(this.SmallChange);
            }
            else
            {
                this.IncrementValue(this.SmallChange);
            }

            e.Handled = true;
        }

        private static void OnScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericBox = d as RadNumericBox;

            numericBox.UpdateInputScope((InputScopeNameValue)e.NewValue);
        }

        private static void OnValueFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadNumericBox box = d as RadNumericBox;
            if (box.IsTemplateApplied)
            {
                box.UpdateTextBoxText();
            }
        }

        private static void OnValueFormatSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadNumericBox box = d as RadNumericBox;
            if (box.IsTemplateApplied)
            {
                box.UpdateTextBoxText();
            }
        }

        private static void OnValueStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadNumericBox box = d as RadNumericBox;
            if (!box.IsInternalPropertyChange)
            {
                throw new InvalidOperationException("ValueString property is readonly.");
            }
        }

        private static bool IsNumericKey(VirtualKey key)
        {
            if (RadNumericBox.IsAzertyKeyboard && key == VirtualKey.Number6 && DeviceTypeHelper.GetDeviceType() != DeviceType.Phone)
            {
                return false;
            }

            if (key >= VirtualKey.Number0 && key <= VirtualKey.Number9)
            {
                return true;
            }

            if (key >= VirtualKey.NumberPad0 && key <= VirtualKey.NumberPad9)
            {
                return true;
            }

            return false;
        }

        private static bool IsNegativeSign(VirtualKey key)
        {
            var isNegativeSign = key == VirtualKey.Subtract || (int)key == DashKey;

            if (RadNumericBox.IsAzertyKeyboard)
            {
                isNegativeSign = isNegativeSign || (key == VirtualKey.Number6 && DeviceTypeHelper.GetDeviceType() != DeviceType.Phone);
            }

            return isNegativeSign;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadNumericBox numericBox = d as RadNumericBox;
            if (numericBox == null || numericBox.IsInternalPropertyChange)
            {
                return;
            }

            numericBox.updatingValue = true;

            double? oldValue = numericBox.valueCache;
            double? newDoubleValue = null;

            if (e.NewValue != null)
            {
                double doubleValue;
                if (!NumericConverter.TryConvertToDouble(e.NewValue, out doubleValue))
                {
                    throw new ArgumentException("The object of type " + e.NewValue.GetType() + " may not be converted to a numerical value.");
                }

                RangeControlBase.VerifyValidDoubleValue(doubleValue);
                newDoubleValue = doubleValue;
            }

            if (numericBox.IsTemplateApplied)
            {
                var coercedValue = numericBox.CoerceValue(newDoubleValue);
                if (coercedValue.HasValue)
                {
                    newDoubleValue = coercedValue.Value;
                }
            }

            numericBox.valueCache = newDoubleValue;

            numericBox.updatingValue = false;

            if (oldValue != numericBox.Value)
            {
                if (numericBox.TextBox != null && numericBox.TextBox.FocusState == FocusState.Unfocused)
                {
                    numericBox.UpdateTextBoxText();
                }
                
                numericBox.OnValueChanged();
            }

            RadNumericBoxAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(numericBox) as RadNumericBoxAutomationPeer;
            if (peer != null && oldValue != null && newDoubleValue != null)
            {
                peer.RaiseValuePropertyChangedEvent(oldValue, newDoubleValue);
            }
        }

        private static void OnAllowNullValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericBox = d as RadNumericBox;
            if (numericBox == null)
            {
                return;
            }

            numericBox.allowNullValueCache = (bool)e.NewValue;

            if (!numericBox.Value.HasValue)
            {
                numericBox.Value = 0d;
            }
        }

        private void UpdateInputScope(InputScopeNameValue inputScopeName)
        {
            if (this.IsTemplateApplied)
            {
                var scope = new InputScope();
                scope.Names.Add(new InputScopeName(inputScopeName));

                this.textBox.InputScope = scope;
            }
        }

        private bool IsDecimalSeparator(VirtualKey key)
        {
            if ((int)key == this.DecimalSeparatorKey)
            {
                return true;
            }

            if (key == VirtualKey.Decimal)
            {
                return true;
            }

            if ((int)key == DotKey)
            {
                return this.currentCulture.NumberFormat.NumberDecimalSeparator == ".";
            }

            if ((int)key == CommaKey)
            {
                return this.currentCulture.NumberFormat.NumberDecimalSeparator == ",";
            }

            return false;
        }

        private bool NeedsWatermark()
        {
            if (!this.IsTemplateApplied)
            {
                return false;
            }

            if (this.textBox.FocusState != FocusState.Unfocused)
            {
                return false;
            }

            return string.IsNullOrEmpty(this.textBox.Text);
        }

        private void OnTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            this.BeginEdit();
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.CommitEdit();
        }

        private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            // marking the event as Handled will prevent the TextBox from updating its Text in case invalid character is pressed.
            e.Handled = !this.PreviewKeyDown(e.Key);
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.UpdateVisualState(true);
            this.ValidateText();
            if (this.GetBindingExpression(ValueProperty)?.ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.PropertyChanged)
            {
                this.Value = this.TryParseValue();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Telerik.UI.Xaml.Controls.Input.RadNumericBox.SetText(System.String)")]
        private void ValidateText()
        {
            string text = this.textBox.Text;
            if (string.IsNullOrEmpty(text))
            {
                if (this.isNegative)
                {
                    this.SetText(this.currentCulture.NumberFormat.NegativeSign);
                }
                return;
            }

            if (text.Length == 1 && (text[0] == '-' || text[0] == '-'))
            {
                if (this.isNegative)
                {
                    this.isNegative = false;
                }
                else
                {
                    this.SetText(string.Empty);
                }
                return;
            }

            double value;
            if (!double.TryParse(text, NumberStyles.Any, this.currentCulture, out value))
            {
                this.UpdateTextBoxText();
            }
            else if (this.isNegative)
            {
                if (value > 0)
                {
                    this.SetText(text.Insert(0, "-"));
                }

                this.isNegative = false;
            }
            else if (this.isDecimal)
            {
                if (!text.Contains(this.currentCulture.NumberFormat.NumberDecimalSeparator))
                {
                    var selectionStartCache = this.textBox.SelectionStart;
                    this.SetText(text.Insert(this.textBox.SelectionStart, this.currentCulture.NumberFormat.NumberDecimalSeparator));
                    this.textBox.SelectionStart = selectionStartCache + 1;
                }

                this.isDecimal = false;
            }
        }

        private void OnDecreaseButtonClick(object sender, RoutedEventArgs e)
        {
            if (!this.decreaseButton.IsTabStop)
            {
                this.KillTextBoxFocus();
            }
            this.DecrementValue(this.SmallChange);
        }

        private void OnIncreaseButtonClick(object sender, RoutedEventArgs e)
        {
            if (!this.decreaseButton.IsTabStop)
            {
                this.KillTextBoxFocus();
            }
            this.IncrementValue(this.SmallChange);
        }

        private void KillTextBoxFocus()
        {
            if (this.textBox.FocusState != FocusState.Unfocused)
            {
                this.CommitEdit();
            }

            // kill text box focus (will commit the current edit)
            this.IsTabStop = true;
            this.Focus(FocusState.Programmatic);
        }

        private void SetText(string text)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            this.textBox.Text = text;
            if (this.textBox.FocusState != FocusState.Unfocused)
            {
                this.textBox.SelectionStart = this.textBox.Text.Length;
            }
            this.ChangePropertyInternally(ValueStringProperty, text);
        }

        private void IncrementValue(double amount)
        {
            if (this.Value.HasValue)
            {
                this.Value += amount;
            }
            else
            {
                this.Value = amount;
            }
        }

        private void DecrementValue(double amount)
        {
            if (this.Value.HasValue)
            {
                this.Value -= amount;
            }
            else
            {
                this.Value = -amount;
            }
        }

        private double? TryParseValue()
        {
            string text = this.textBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                double value;
                if (double.TryParse(text, NumberStyles.Any, this.currentCulture, out value))
                {
                    return value;
                }
            }

            double? defaultValue;
            if (this.AllowNullValue)
            {
                defaultValue = null;
            }
            else
            {
                defaultValue = 0d;
            }

            return defaultValue;
        }

        private void UpdateTextBoxText()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            double? value = this.Value;
            string text;

            if (value.HasValue)
            {
                if (!this.isEditing)
                {
                    string format = this.ValueFormat;

                    if (this.ValueFormatSelector != null)
                    {
                        format = this.ValueFormatSelector.GetFormat(value.Value);
                    }

                    text = string.Format(this.currentCulture, format, value.Value);
                }
                else
                {
                    text = value.Value.ToString(this.currentCulture);
                }
            }
            else
            {
                text = string.Empty;
            }

            this.SetText(text);
        }

        private double? CoerceValue(double? newValue)
        {
            if (newValue == null && this.allowNullValueCache)
            {
                return null;
            }

            var min = this.Minimum;
            var max = this.Maximum;
            double? coercedNewValue = newValue;

            if (coercedNewValue < min)
            {
                coercedNewValue = min;
            }
            else if (coercedNewValue > max)
            {
                coercedNewValue = max;
            }

            if (coercedNewValue != newValue)
            {
                this.ChangePropertyInternally(ValueProperty, coercedNewValue);
                this.valueCache = coercedNewValue;
                if (!this.updatingValue)
                {
                    this.OnValueChanged();
                }
            }

            return coercedNewValue;
        }
    }
}