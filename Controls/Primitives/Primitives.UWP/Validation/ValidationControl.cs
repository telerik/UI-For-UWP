using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Visual representation of errors provided through <see cref="INotifyDataErrorInfo"/> interface.
    /// </summary>
    [TemplatePart(Name = "PART_ErrorMessages", Type = typeof(Popup))]
    public class ValidationControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="RelativePositionOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RelativePositionOffsetProperty =
            DependencyProperty.Register(nameof(RelativePositionOffset), typeof(Point), typeof(ValidationControl), new PropertyMetadata(new Point(5, -5)));

        /// <summary>
        /// Identifies the <see cref="ControlPeer"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ControlPeerProperty =
            DependencyProperty.Register(nameof(ControlPeer), typeof(FrameworkElement), typeof(ValidationControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DataItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataItemProperty =
            DependencyProperty.Register(nameof(DataItem), typeof(object), typeof(ValidationControl), new PropertyMetadata(null, OnDataItemChanged));

        /// <summary>
        /// Identifies the <see cref="PropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(ValidationControl), new PropertyMetadata(null, OnPropertyNameChanged));

        /// <summary>
        /// Identifies the <see cref="ValidateOnErrors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidateOnErrorsProperty =
            DependencyProperty.Register(nameof(ValidateOnErrors), typeof(bool), typeof(ValidationControl), new PropertyMetadata(true, OnValidateOnErrorsChanged));

        /// <summary>
        /// Identifies the <see cref="IsValid"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(ValidationControl), new PropertyMetadata(true, OnIsValidChanged));

        private INotifyDataErrorInfo dataErrorInfo;
        private INotifyPropertyChanged notifyPropertyChangedItem;

        private ObservableCollection<object> errors;

        private Popup errorPopup;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationControl" /> class.
        /// </summary>
        public ValidationControl()
        {
            this.DefaultStyleKey = typeof(ValidationControl);

            this.errors = new ObservableCollection<object>();
        }

        /// <summary>
        /// Occurs when validating item.
        /// </summary>
        public event EventHandler<ValidatingEventArgs> Validating;

        /// <summary>
        /// Gets or sets the relative offset from <see cref="ControlPeer"/> control.
        /// </summary>
        public Point RelativePositionOffset
        {
            get { return (Point)this.GetValue(RelativePositionOffsetProperty); }
            set { this.SetValue(RelativePositionOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the control that will be used to position validation control.
        /// </summary>
        public FrameworkElement ControlPeer
        {
            get { return (FrameworkElement)this.GetValue(ControlPeerProperty); }
            set { this.SetValue(ControlPeerProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether control is in valid state.
        /// </summary>
        public bool IsValid
        {
            get { return (bool)this.GetValue(IsValidProperty); }
            set { this.SetValue(IsValidProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether control should trigger validation when error occurs.
        /// </summary>
        public bool ValidateOnErrors
        {
            get { return (bool)this.GetValue(ValidateOnErrorsProperty); }
            set { this.SetValue(ValidateOnErrorsProperty, value); }
        }

        /// <summary>
        /// Gets the collection of current errors.
        /// </summary>
        public ObservableCollection<object> Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Gets or sets the data item that if implements <see cref="INotifyDataErrorInfo"/> interface will be used as source for errors.
        /// </summary>
        public object DataItem
        {
            get { return (object)this.GetValue(DataItemProperty); }
            set { this.SetValue(DataItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the name of the property that will be validated.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return (string)this.GetValue(PropertyNameProperty); }
            set { this.SetValue(PropertyNameProperty, value); }
        }

        private INotifyDataErrorInfo DataErrorInfo
        {
            get
            {
                return this.dataErrorInfo;
            }
            set
            {
                if (this.dataErrorInfo != null)
                {
                    this.dataErrorInfo.ErrorsChanged -= this.OnDataItemInfoErrorsChanged;
                }

                this.dataErrorInfo = value;

                if (this.dataErrorInfo != null)
                {
                    this.dataErrorInfo.ErrorsChanged += this.OnDataItemInfoErrorsChanged;
                }
            }
        }

        private INotifyPropertyChanged NotifyPropertyChangedItem
        {
            get
            {
                return this.notifyPropertyChangedItem;
            }
            set
            {
                if (this.notifyPropertyChangedItem != null)
                {
                    this.notifyPropertyChangedItem.PropertyChanged -= this.OnItemNotifyPropertyChanged;
                }

                this.notifyPropertyChangedItem = value;

                if (this.notifyPropertyChangedItem != null)
                {
                    this.notifyPropertyChangedItem.PropertyChanged += this.OnItemNotifyPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Validates the data updates the <see cref="Errors"/> collection.
        /// </summary>
        /// <param name="callAsync">Whether the validation will be executed asynchronously.</param>
        public void Validate(bool callAsync = true)
        {
            this.ValidateCore(callAsync, true);
        }

        /// <summary>
        /// Called to raise <see cref="Validating"/> event.
        /// </summary>
        /// <param name="currentErrors">The current errors.</param>
        protected virtual ValidatingEventArgs OnValidating(IList<object> currentErrors)
        {
            var arg = new ValidatingEventArgs(currentErrors);

            if (this.Validating != null)
            {
                this.Validating(this, arg);
            }

            return arg;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate"/> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            this.errorPopup = this.GetTemplatePartField<Popup>("PART_ErrorMessages");

            return this.errorPopup != null && base.ApplyTemplateCore();
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            if (this.errorPopup.IsOpen)
            {
                this.errorPopup.LayoutUpdated += this.OnPopupLayoutUpdated;
            }
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            return this.IsValid ? "Valid" : "Invalid";
        }

        private static void OnIsValidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationControl;
            control.UpdateVisualState(true);
        }

        private static void OnDataItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationControl;

            control.DataErrorInfo = e.NewValue as INotifyDataErrorInfo;

            control.NotifyPropertyChangedItem = e.NewValue as INotifyPropertyChanged;

            control.ValidateCore();
        }

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationControl;
            control.ValidateCore();
        }

        private static void OnValidateOnErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ValidationControl;
            control.ValidateCore();
        }

        private void OnDataItemInfoErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.PropertyName) || e.PropertyName == this.PropertyName)
            {
                this.ValidateCore();
            }
        }

        private void OnItemNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.PropertyName) || e.PropertyName == this.PropertyName)
            {
                this.ValidateCore();
            }
        }

        private void ValidateCore(bool async = true, bool forceValidate = false)
        {
            if ((this.IsTemplateApplied && this.ValidateOnErrors) || forceValidate)
            {
                var asyncDataErrorInfo = this.DataErrorInfo as IAsyncDataErrorInfo;
                if (asyncDataErrorInfo != null && !async)
                {
                    asyncDataErrorInfo.ValidateAsync(this.PropertyName).Wait();
                }

                this.Errors.Clear();

                List<object> newErrors = new List<object>();
                if (this.DataErrorInfo != null)
                {
                    newErrors = this.DataErrorInfo.GetErrors(this.PropertyName).OfType<object>().ToList();
                }

                var args = this.OnValidating(newErrors);

                foreach (var error in args.Errors)
                {
                    this.Errors.Add(error);
                }

                this.IsValid = this.errors.Count == 0;

                this.errorPopup.IsOpen = !this.IsValid;

                if (this.errorPopup.IsOpen)
                {
                    this.errorPopup.LayoutUpdated += this.OnPopupLayoutUpdated;
                }
            }
        }

        private void OnPopupLayoutUpdated(object sender, object e)
        {
            this.errorPopup.LayoutUpdated -= this.OnPopupLayoutUpdated;

            if (this.ControlPeer != null)
            {
                var transform = this.ControlPeer.TransformToVisual(this.errorPopup);
                var rect = transform.TransformBounds(new Rect(0, 0, this.ControlPeer.ActualWidth, this.ControlPeer.ActualHeight));

                this.errorPopup.VerticalOffset = rect.Bottom + this.RelativePositionOffset.Y;
                this.errorPopup.HorizontalOffset = rect.X + this.RelativePositionOffset.X;
            }
        }
    }
}
