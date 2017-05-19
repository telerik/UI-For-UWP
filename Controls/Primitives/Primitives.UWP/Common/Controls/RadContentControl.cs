using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls
{
    /// <summary>
    /// Extends the base <see cref="Windows.UI.Xaml.Controls.ContentControl"/>.
    /// Wraps basic routed events like Loaded and Unloaded in virtual methods and expose common properties like IsLoaded and IsFocused.
    /// </summary>
    public class RadContentControl : ContentControl
    {
        /// <summary>
        /// Defines the IsFocused property.
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(nameof(IsFocused), typeof(bool), typeof(RadContentControl), new PropertyMetadata(false, OnIsFocusedChanged));

        private bool isFocused;
        private string currentVisualState;
        private bool isLoaded;
        private bool isTemplateApplied;
        private byte visualStateUpdateLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadContentControl"/> class.
        /// </summary>
        public RadContentControl()
        {
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
            this.IsEnabledChanged += this.OnIsEnabledChanged;

            this.currentVisualState = string.Empty;
        }

        /// <summary>
        /// Gets the current visual state of the control.
        /// </summary>
        public string CurrentVisualState
        {
            get
            {
                return this.currentVisualState;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is currently loaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return this.isLoaded;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/> method has been called for this instance.
        /// </summary>
        public bool IsTemplateApplied
        {
            get
            {
                return this.isTemplateApplied;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is currently Focused (has the keyboard input).
        /// </summary>
        public bool IsFocused
        {
            get
            {
                return this.isFocused;
            }
            protected set
            {
                this.SetValue(IsFocusedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control template parts
        /// have been successfully acquired after the OnApplyTemplate call.
        /// </summary>
        protected internal virtual bool IsProperlyTemplated
        {
            get
            {
                return this.isTemplateApplied;
            }
        }

        /// <summary>
        /// Locks any visual state update. Useful when performing batch operations.
        /// </summary>
        public void BeginVisualStateUpdate()
        {
            this.visualStateUpdateLock++;
        }

        /// <summary>
        /// Resumes visual state update and optionally re-evaluates the current visual state.
        /// </summary>
        /// <param name="update">True to re-evaluate the current visual state, false otherwise.</param>
        /// <param name="animate">True to use animations when updating visual state, false otherwise.</param>
        public void EndVisualStateUpdate(bool update, bool animate)
        {
            if (this.visualStateUpdateLock > 0)
            {
                this.visualStateUpdateLock--;
            }

            if (update && this.visualStateUpdateLock == 0)
            {
                this.UpdateVisualState(animate);
            }
        }

        /// <summary>
        /// Re-evaluates the current visual state for the control and updates it if necessary.
        /// </summary>
        /// <param name="animate">True to use transitions during state update, false otherwise.</param>
        protected internal void UpdateVisualState(bool animate)
        {
            if (!this.CanUpdateVisualState())
            {
                return;
            }

            string state = this.ComposeVisualStateName();
            if (state != this.currentVisualState)
            {
                this.currentVisualState = state;
                this.SetVisualState(state, animate);
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.isTemplateApplied = true;
            this.currentVisualState = string.Empty;

            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Applies the specified visual state as current.
        /// </summary>
        /// <param name="state">The new visual state.</param>
        /// <param name="animate">True to use transitions, false otherwise.</param>
        protected virtual void SetVisualState(string state, bool animate)
        {
            string[] states = state.Split(RadControl.VisualStateDelimiter);
            foreach (string visualState in states)
            {
                VisualStateManager.GoToState(this, visualState, animate);
            }
        }

        /// <summary>
        /// Determines whether the current visual state may be updated.
        /// </summary>
        protected virtual bool CanUpdateVisualState()
        {
            return this.isTemplateApplied && this.visualStateUpdateLock == 0;
        }

        /// <summary>
        /// Occurs when this object is no longer connected to the main object tree.
        /// </summary>
        protected virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = false;
        }

        /// <summary>
        /// Occurs when a System.Windows.FrameworkElement has been constructed and added to the object tree.
        /// </summary>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected virtual string ComposeVisualStateName()
        {
            return this.IsEnabled ? "Normal" : "Disabled";
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            this.IsFocused = true;
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            this.IsFocused = false;
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.RadContentControlAutomationPeer(this);
        }

        /// <summary>
        /// Gets a template part for the provided name and type and throws an exception if it is missing.
        /// </summary>
        /// <typeparam name="T">The type of the template part.</typeparam>
        /// <param name="partName">The name of the template part.</param>
        /// <param name="throwException">Determines if the method should throw an exception for a missing part.</param>
        /// <returns>Returns the specified template part or throws an exception if it is missing.</returns>
        protected T GetTemplatePart<T>(string partName, bool throwException = true) where T : class
        {
            T templatePart = this.GetTemplateChild(partName) as T;

            if (templatePart == null && throwException)
            {
                throw new MissingTemplatePartException(partName, typeof(T));
            }

            return templatePart;
        }

        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadContentControl control = d as RadContentControl;
            control.isFocused = (bool)e.NewValue;
            control.UpdateVisualState(control.isLoaded);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateVisualState(this.IsLoaded);
        }
    }
}