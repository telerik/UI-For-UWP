﻿using System;
using System.Windows.Input;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Primitives.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the user interface that represents the built-in filtering within a <see cref="RadDataGrid"/> instance.
    /// </summary>
    [TemplatePart(Name = "PART_LogicalOperatorCombo", Type = typeof(DataGridFilterComboBox))]
    [TemplateVisualState(Name = "NormalExpandable", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "NormalNotExpandable", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "CommonStates")]
    public class DataGridFilteringFlyout : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="FirstFilterControl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstFilterControlProperty =
            DependencyProperty.Register("FirstFilterControl", typeof(IFilterControl), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SecondFilterControl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondFilterControlProperty =
            DependencyProperty.Register("SecondFilterControl", typeof(IFilterControl), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(DataGridFilteringFlyout), new PropertyMetadata(false, OnIsExpandedChanged));

        /// <summary>
        /// Identifies the <see cref="FilterGlyphWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterGlyphWidthProperty =
            DependencyProperty.Register("FilterGlyphWidth", typeof(double), typeof(DataGridFilteringFlyout), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="FilterGlyphHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterGlyphHeightProperty =
            DependencyProperty.Register("FilterGlyphHeight", typeof(double), typeof(DataGridFilteringFlyout), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="FilterButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilterButtonStyleProperty =
            DependencyProperty.Register("FilterButtonStyle", typeof(Style), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ClearFilterButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClearFilterButtonStyleProperty =
            DependencyProperty.Register("ClearFilterButtonStyle", typeof(Style), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ExpandCollapseButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandCollapseButtonStyleProperty =
            DependencyProperty.Register("ExpandCollapseButtonStyle", typeof(Style), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DisplayMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(FilteringFlyoutDisplayMode), typeof(DataGridFilteringFlyout), new PropertyMetadata(FilteringFlyoutDisplayMode.Fill, OnDisplayModeChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        public static readonly DependencyProperty FilterCommandProperty =
            DependencyProperty.Register("FilterCommand", typeof(ICommand), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        public static readonly DependencyProperty ExpandCommandProperty =
            DependencyProperty.Register("ExpandCommand", typeof(ICommand), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        public static readonly DependencyProperty ClearFilterCommandProperty =
            DependencyProperty.Register("ClearFilterCommand", typeof(ICommand), typeof(DataGridFilteringFlyout), new PropertyMetadata(null));

        private ComboBox logicalOperatorCombo;
        private RadDataGrid owner;
        private FilterButtonTapContext context;

        public ICommand FilterCommand
        {
            get { return (ICommand)GetValue(FilterCommandProperty); }
            set { SetValue(FilterCommandProperty, value); }
        }

        public ICommand ExpandCommand
        {
            get { return (ICommand)GetValue(ExpandCommandProperty); }
            set { SetValue(ExpandCommandProperty, value); }
        }

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public ICommand ClearFilterCommand
        {
            get { return (ICommand)GetValue(ClearFilterCommandProperty); }
            set { SetValue(ClearFilterCommandProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFilteringFlyout" /> class.
        /// </summary> 
        public DataGridFilteringFlyout()
        {
            this.DefaultStyleKey = typeof(DataGridFilteringFlyout);
            this.FilterCommand = new DataGridFilterUICommand(this, DataGridFilterUIActionCommandID.Filter);
            this.ExpandCommand = new DataGridFilterUICommand(this, DataGridFilterUIActionCommandID.ExpandCollapse);
            this.CancelCommand = new DataGridFilterUICommand(this, DataGridFilterUIActionCommandID.Cancel);
            this.ClearFilterCommand = new DataGridFilterUICommand(this, DataGridFilterUIActionCommandID.ClearFilter);
        }

        /// <summary>
        /// Gets or sets the style of the flyout header. Not available for <see cref="DisplayMode"/> = <see cref="FilteringFlyoutDisplayMode.Inline"/>.
        /// </summary>
        public Style HeaderStyle
        {
            get
            {
                return (Style)this.GetValue(HeaderStyleProperty);
            }
            set
            {
                this.SetValue(HeaderStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that controls the appearance of the FilterButton template part.
        /// <remarks>
        /// The style should target the <see cref="Telerik.UI.Xaml.Controls.Primitives.Common.InlineButton"/> type.
        /// </remarks>
        /// </summary>
        public Style FilterButtonStyle
        {
            get
            {
                return this.GetValue(FilterButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(FilterButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that controls the appearance of the ClearFilterButton template part.
        /// <remarks>
        /// The style should target the <see cref="Telerik.UI.Xaml.Controls.Primitives.Common.InlineButton"/> type.
        /// </remarks>
        /// </summary>
        public Style ClearFilterButtonStyle
        {
            get
            {
                return this.GetValue(ClearFilterButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(ClearFilterButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that controls the appearance of the ExpandCollapseButton template part.
        /// <remarks>
        /// The style should target the <see cref="Telerik.UI.Xaml.Controls.Primitives.Common.InlineButton"/> type.
        /// </remarks>
        /// </summary>
        public Style ExpandCollapseButtonStyle
        {
            get
            {
                return this.GetValue(ExpandCollapseButtonStyleProperty) as Style;
            }
            set
            {
                this.SetValue(ExpandCollapseButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the filter glyph template part.
        /// </summary>
        public double FilterGlyphHeight
        {
            get
            {
                return (double)this.GetValue(FilterGlyphHeightProperty);
            }
            set
            {
                this.SetValue(FilterGlyphHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width the filter glyph template part.
        /// </summary>
        public double FilterGlyphWidth
        {
            get
            {
                return (double)this.GetValue(FilterGlyphWidthProperty);
            }
            set
            {
                this.SetValue(FilterGlyphWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is expanded (displaying the second filtering controls) or not.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(IsExpandedProperty);
            }
            set
            {
                this.SetValue(IsExpandedProperty, value);
            }
        }

        public RadDataGrid Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IFilterControl"/> instance that represents the first filter in the flyout.
        /// </summary>
        public IFilterControl FirstFilterControl
        {
            get
            {
                return this.GetValue(FirstFilterControlProperty) as IFilterControl;
            }
            set
            {
                this.SetValue(FirstFilterControlProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IFilterControl"/> instance that represents the second filter in the flyout.
        /// </summary>
        public IFilterControl SecondFilterControl
        {
            get
            {
                return this.GetValue(SecondFilterControlProperty) as IFilterControl;
            }
            set
            {
                this.SetValue(SecondFilterControlProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the display mode of the flyout.
        /// </summary>
        internal FilteringFlyoutDisplayMode DisplayMode
        {
            get
            {
                return (FilteringFlyoutDisplayMode)this.GetValue(DisplayModeProperty);
            }
            set
            {
                this.SetValue(DisplayModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the operator combo box. Exposed for testing purposes only.
        /// </summary>
        internal ComboBox OperatorCombo
        {
            get
            {
                return this.logicalOperatorCombo;
            }
        }

        internal FilterButtonTapContext Context
        {
            get
            {
                return this.context;
            }
        }

        internal void Initialize(RadDataGrid newOwner, FilterButtonTapContext newContext)
        {
            this.owner = newOwner;
            this.context = newContext;
            this.FirstFilterControl = newContext.FirstFilterControl;
            this.SecondFilterControl = newContext.SecondFilterControl;

            if (this.context.FirstFilterControl != null)
            {
                this.context.FirstFilterControl.IsFirst = true;
                this.context.FirstFilterControl.AssociatedDescriptor = this.context.AssociatedDescriptor;
            }
            if (this.context.SecondFilterControl != null)
            {
                this.context.SecondFilterControl.AssociatedDescriptor = this.context.AssociatedDescriptor;
            }
        }

        internal void Close()
        {
            var id = this.DisplayMode == FilteringFlyoutDisplayMode.Inline ? DataGridFlyoutId.FilterButton : DataGridFlyoutId.FlyoutFilterButton;
            this.owner.ContentFlyout.Hide(id);
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.logicalOperatorCombo = this.GetTemplatePartField<ComboBox>("PART_LogicalOperatorCombo");
            applied = applied && this.logicalOperatorCombo != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            this.logicalOperatorCombo.Items.Add(GridLocalizationManager.Instance.GetString(LogicalOperator.And.ToString()));
            this.logicalOperatorCombo.Items.Add(GridLocalizationManager.Instance.GetString(LogicalOperator.Or.ToString()));

            var compositeDescriptor = this.context.AssociatedDescriptor as CompositeFilterDescriptor;
            if (compositeDescriptor != null && this.context.SecondFilterControl != null)
            {
                this.logicalOperatorCombo.SelectedIndex = compositeDescriptor.Operator == LogicalOperator.And ? 0 : 1;
                this.IsExpanded = true;
            }
            else
            {
                this.logicalOperatorCombo.SelectedIndex = 0;
            }

            base.OnTemplateApplied();

            this.UpdateStyles();
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.IsExpanded)
            {
                return "Expanded";
            }

            return this.SecondFilterControl == null ? "NormalNotExpandable" : "NormalExpandable";
        }

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as DataGridFilteringFlyout;
            flyout.UpdateStyles();
        }

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as DataGridFilteringFlyout;
            flyout.UpdateStyles();
        }

        Thickness? borderThickness;

        private void UpdateStyles()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            switch (this.DisplayMode)
            {
                case FilteringFlyoutDisplayMode.Fill:

                    this.borderThickness = this.BorderThickness;
                   
                    VisualStateManager.GoToState(this, this.IsExpanded ? "FillExpanded" : "FillCollased", false);
                    this.Width = Math.Max(this.MinWidth, this.owner.ActualWidth);

                    if (this.IsExpanded)
                    {
                        this.Height = this.owner.ActualHeight;
                        this.BorderThickness = new Thickness(0);
                    }
                    else
                    {
                        this.BorderThickness = new Thickness(0,0,0,2);
                    }

                    break;
                case FilteringFlyoutDisplayMode.Flyout:
                    if (this.borderThickness.HasValue)
                    {
                        this.BorderThickness = this.borderThickness.Value;
                    }
                    this.MaxWidth = 400;
                    VisualStateManager.GoToState(this, "Flyout", false);
                    break;
                case FilteringFlyoutDisplayMode.Inline:
                    if (this.borderThickness.HasValue)
                    {
                        this.BorderThickness = this.borderThickness.Value;
                    }
                    VisualStateManager.GoToState(this, "Inline", false);
                    break;
            }
        }
    }



    public enum DataGridFilterUIActionCommandID
    {
        Filter,
        ExpandCollapse,
        Cancel,
        ClearFilter,
    }

    public class DataGridFilterUICommand : ICommand
    {
        private DataGridFilteringFlyout owner;
        private DataGridFilterUIActionCommandID id;

        public DataGridFilterUICommand(DataGridFilteringFlyout owner, DataGridFilterUIActionCommandID id)
        {
            this.owner = owner;
            this.id = id;
        }

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            switch (this.id)
            {
                case DataGridFilterUIActionCommandID.Cancel:
                    this.Cancel(); return;
                case DataGridFilterUIActionCommandID.ClearFilter:
                    this.Clear();
                    return;
                case DataGridFilterUIActionCommandID.ExpandCollapse:
                    ExpandCollapse();
                    return;
                case DataGridFilterUIActionCommandID.Filter:
                    this.Filter();
                    return;
            }
        }

        private void Filter()
        {
            if (this.owner.Context.AssociatedDescriptor != null)
            {
                this.owner.Owner.FilterDescriptors.Remove(this.owner.Context.AssociatedDescriptor);
            }

            FilterDescriptorBase result;
            if (this.owner.IsExpanded)
            {
                CompositeFilterDescriptor compositeDescriptor = new CompositeFilterDescriptor();
                compositeDescriptor.Operator = (LogicalOperator)this.owner.OperatorCombo.SelectedIndex;
                var firstDescriptor = this.owner.FirstFilterControl.BuildDescriptor();
                var secondDescriptor = this.owner.SecondFilterControl == null ? null : this.owner.SecondFilterControl.BuildDescriptor();

                if (firstDescriptor != null)
                {
                    compositeDescriptor.Descriptors.Add(firstDescriptor);
                }
                if (secondDescriptor != null)
                {
                    compositeDescriptor.Descriptors.Add(secondDescriptor);
                }

                result = compositeDescriptor;
            }
            else
            {
                result = this.owner.FirstFilterControl.BuildDescriptor();
            }

            FilterRequestedContext filterContext = new FilterRequestedContext();
            filterContext.Descriptor = result;
            filterContext.Column = this.owner.Context.Column;
            filterContext.IsFiltering = true;
            this.owner.Owner.CommandService.ExecuteCommand(CommandId.FilterRequested, filterContext);

            this.owner.Close();
        }

        private void Clear()
        {
            FilterRequestedContext filterContext = new FilterRequestedContext();
            filterContext.IsFiltering = false;
            filterContext.Descriptor = this.owner.Context.AssociatedDescriptor;
            filterContext.Column = this.owner.Context.Column;
            this.owner.Owner.CommandService.ExecuteCommand(CommandId.FilterRequested, filterContext);

            this.owner.Close();
        }

        private void ExpandCollapse()
        {
            if (this.owner.SecondFilterControl != null)
            {
                this.owner.IsExpanded ^= true;
                this.owner.UpdateVisualState(true);
            }
        }

        private void Cancel()
        {
            var id = this.owner.DisplayMode == FilteringFlyoutDisplayMode.Inline ? DataGridFlyoutId.FilterButton : DataGridFlyoutId.FlyoutFilterButton;
            this.owner.Owner.ContentFlyout.Hide(id);
        }
    }
}
