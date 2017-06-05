using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls
{
    /// <summary>
    /// Defines the base class for all Telerik-specific controls. Encapsulates common routines and properties.
    /// </summary>
    public abstract class RadControl : Control
    {
        internal const char VisualStateDelimiter = ',';

        // For test purposes only.
        internal static bool IsInTestMode;

        private bool isUnloaded; // determines whether the Unload event has been received
        private bool isLoaded;
        private bool isLoading; // occurs while the LoadCore virtual method is executed up in the hierachy chain.
        private bool wasUnloaded; // determines whether the control is loading after it has been unloaded from the visual tree.
        private bool isTemplateApplied;
        private byte internalPropertyChange;
        private byte visualStateUpdateLock;
        private string currentVisualState = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadControl"/> class.
        /// </summary>
        protected RadControl()
        {
            this.IsEnabledChanged += this.OnIsEnabledChanged;
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
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

        internal bool IsInternalPropertyChange
        {
            get
            {
                return this.internalPropertyChange > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="E:Loaded"/> event is handled and the <see cref="LoadCore"/> routine is passed.
        /// </summary>
        protected internal bool IsLoaded
        {
            get
            {
                return this.isLoaded || IsInTestMode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is currently in a process of being loaded on the visual tree. 
        /// This flag indicates that currently the LoadCore routine is executed and the control is still not completely loaded.
        /// </summary>
        protected internal bool IsLoading
        {
            get
            {
                return this.isLoading;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Unloaded event has been received.
        /// </summary>
        protected internal bool IsUnloaded
        {
            get
            {
                return this.isUnloaded;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control has been unloaded from the visual tree.
        /// </summary>
        protected internal bool WasUnloaded
        {
            get
            {
                return this.wasUnloaded;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="OnApplyTemplate"/> method and the <see cref="ApplyTemplateCore"/> routine is passed.
        /// </summary>
        protected internal bool IsTemplateApplied
        {
            get
            {
                return this.isTemplateApplied;
            }
        }

        internal bool InvokeAsync(DispatchedHandler action)
        {
            return this.InvokeAsync(CoreDispatcherPriority.Normal, action);
        }

        internal bool InvokeAsync(CoreDispatcherPriority priority, DispatchedHandler action)
        {
            if (this.isUnloaded || !this.isTemplateApplied)
            {
                return false;
            }

            // For some reason a COM exception is raised when trying to access the Dispatcher ar design-time
            if (DesignMode.DesignModeEnabled)
            {
                return false;
            }

            var suppressionVariable = this.Dispatcher.RunAsync(priority, action);
            return true;
        }
        
        internal void ChangePropertyInternally(DependencyProperty property, object value)
        {
            this.internalPropertyChange++;
            this.SetValue(property, value);
            this.internalPropertyChange--;
        }

        internal T GetTemplatePartField<T>(string name) where T : DependencyObject
        {
            T part = this.GetTemplateChild(name) as T;
            if (part == null && !DesignMode.DesignModeEnabled)
            {
                throw new MissingTemplatePartException(name, typeof(T));
            }

            return part;
        }
        
        /// <summary>
        /// Resumes visual state update and optionally re-evaluates the current visual state.
        /// </summary>
        /// <param name="update">True to re-evaluate the current visual state, false otherwise.</param>
        /// <param name="animate">True to use animations when updating visual state, false otherwise.</param>
        protected internal void EndVisualStateUpdate(bool update, bool animate)
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
        protected internal virtual void UpdateVisualState(bool animate)
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
        /// Locks any visual state update. Useful when performing batch operations.
        /// </summary>
        protected internal void BeginVisualStateUpdate()
        {
            this.visualStateUpdateLock++;
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Automation.Peers.RadControlAutomationPeer(this);
        }

        /// <summary>
        /// Applies the specified visual state as current.
        /// </summary>
        /// <param name="state">The new visual state.</param>
        /// <param name="animate">True to use transitions, false otherwise.</param>
        protected virtual void SetVisualState(string state, bool animate)
        {
            if (string.IsNullOrEmpty(state))
            {
                return;
            }

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
            return this.isTemplateApplied && (this.visualStateUpdateLock == 0);
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected virtual string ComposeVisualStateName()
        {
            return this.IsEnabled ? "Normal" : "Disabled";
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            var baseSize = base.MeasureOverride(availableSize);

            if (this.HorizontalAlignment == HorizontalAlignment.Stretch && !double.IsInfinity(availableSize.Width))
            {
                baseSize.Width = availableSize.Width;
            }
            if (this.VerticalAlignment == VerticalAlignment.Stretch && !double.IsInfinity(availableSize.Height))
            {
                baseSize.Height = availableSize.Height;
            }

            double width = Math.Max(this.MinWidth, baseSize.Width);
            width = Math.Min(width, this.MaxWidth);

            double height = Math.Max(this.MinHeight, baseSize.Height);
            height = Math.Min(height, this.MaxHeight);

            return new Size(width, height);
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.VerifyTemplateApplied();

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            if (this.isTemplateApplied)
            {
                this.UnapplyTemplateCore();
            }

            base.OnApplyTemplate();

            this.isTemplateApplied = this.ApplyTemplateCore();

            if (this.isTemplateApplied)
            {
                this.OnTemplateApplied();
            }
        }

        /// <summary>
        /// Removes the current control template. Occurs when a template has already been applied and a new one is applied.
        /// </summary>
        protected virtual void UnapplyTemplateCore()
        {
        }

        /// <summary>
        /// Called when the IsEnabled property has changed.
        /// </summary>
        protected virtual void OnIsEnabledChanged(bool newValue, bool oldValue)
        {
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected virtual void OnTemplateApplied()
        {
            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate"/> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected virtual bool ApplyTemplateCore()
        {
            return true;
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected virtual void LoadCore()
        {
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic after the control has been successfully Loaded.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected virtual void UnloadCore()
        {
        }

        private void VerifyTemplateApplied()
        {
            if (DesignMode.DesignModeEnabled)
            {
                // TODO: This may happen in Blend
                return;
            }

            if (!this.IsTemplateApplied)
            {
                throw new TemplateNotAppliedException(this.GetType(), this.DefaultStyleKey);
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.OnIsEnabledChanged((bool)e.NewValue, (bool)e.OldValue);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isUnloaded = false;
            this.isLoading = true;

            this.LoadCore();

            this.isLoading = false;
            this.isLoaded = true;
            this.wasUnloaded = false;
            this.OnLoaded();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = false;
            this.UnloadCore();
            this.isUnloaded = true;
            this.wasUnloaded = true;
        }
    }
}