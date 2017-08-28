using System;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Data.DataForm.Commands;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a RadDataForm control.
    /// </summary>
    [TemplatePart(Name = "PART_ChildrensPanelPresenter", Type = typeof(ContentControl))]
    public class RadDataForm : RadControl, IDataFormView
    {
        /// <summary>
        /// Identifies the <see cref="LayoutDefinition"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LayoutDefinitionProperty =
            DependencyProperty.Register(nameof(LayoutDefinition), typeof(DataFormLayoutDefinition), typeof(RadDataForm), new PropertyMetadata(null, OnLayoutDefinitionChanged));

        /// <summary>
        /// Identifies the <see cref="ValidationMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ValidationModeProperty =
            DependencyProperty.Register(nameof(ValidationMode), typeof(ValidationMode), typeof(RadDataForm), new PropertyMetadata(ValidationMode.OnCommit));

        /// <summary>
        /// Identifies the <see cref="PropertyIteratorMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty PropertyIteratorModeProperty =
            DependencyProperty.Register(nameof(PropertyIteratorMode), typeof(PropertyIteratorMode), typeof(RadDataForm), new PropertyMetadata(PropertyIteratorMode.All, OnPropertyIteratorModeChanged));

        /// <summary>
        /// Identifies the <see cref="Item"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item), typeof(object), typeof(RadDataForm), new PropertyMetadata(null, OnItemChanged));

        /// <summary>
        /// Identifies the <see cref="EditorFactory"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EditorFactoryProperty =
            DependencyProperty.Register(nameof(EditorFactory), typeof(EditorFactory), typeof(RadDataForm), new PropertyMetadata(null, OnEditorFactoryChanged));

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RadDataForm), new PropertyMetadata(false, OnIsReadOnlyChanged));

        /// <summary>
        /// Identifies the <see cref="GroupHeaderTemplateSelector"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty GroupHeaderTemplateSelectorProperty =
            DependencyProperty.Register(nameof(GroupHeaderTemplateSelector), typeof(DataTemplateSelector), typeof(RadDataForm), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="EditorStyleSelector"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EditorStyleSelectorProperty =
            DependencyProperty.Register(nameof(EditorStyleSelector), typeof(StyleSelector), typeof(RadDataForm), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="EntityProvider"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EntityProviderProperty =
            DependencyProperty.Register(nameof(EntityProvider), typeof(EntityProvider), typeof(RadDataForm), new PropertyMetadata(null, OnEntityProviderChanged));

        /// <summary>
        /// Identifies the <see cref="CommitMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CommitModeProperty =
            DependencyProperty.Register(nameof(CommitMode), typeof(CommitMode), typeof(RadDataForm), new PropertyMetadata(CommitMode.Immediate, OnCommitModeChanged));
        
        internal ContentControl childrensPanelPresenter;
        internal Panel RootPanel;
        private TransactionService transactionService;
        private PropertyIteratorMode iteratorMode;
        private CommandService commandService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadDataForm"/> class.
        /// </summary>
        public RadDataForm()
        {
            this.DefaultStyleKey = typeof(RadDataForm);
            this.Model = new DataFormModel(this);
            this.transactionService = new TransactionService(this);
            this.commandService = new CommandService(this);
        }

        /// <summary>
        /// Gets or sets the item of the <see cref="RadDataForm"/>.
        /// </summary>
        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { this.SetValue(ItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout definition of the control.
        /// </summary>
        public DataFormLayoutDefinition LayoutDefinition
        {
            get { return (DataFormLayoutDefinition)GetValue(LayoutDefinitionProperty); }
            set { this.SetValue(LayoutDefinitionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="EntityProvider"/> of the control.
        /// </summary>
        public EntityProvider EntityProvider
        {
            get { return (EntityProvider)GetValue(EntityProviderProperty); }
            set { this.SetValue(EntityProviderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="ValidationMode"/> of the control.
        /// </summary>
        public ValidationMode ValidationMode
        {
            get { return (ValidationMode)GetValue(ValidationModeProperty); }
            set { this.SetValue(ValidationModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the factory used for generation of the editors used by the control.
        /// </summary>
        public EditorFactory EditorFactory
        {
            get { return (EditorFactory)GetValue(EditorFactoryProperty); }
            set { this.SetValue(EditorFactoryProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Windows.UI.Xaml.Controls.DataTemplateSelector"/> used to choose DataTemplate to display the group headers that are part of the control. 
        /// This is a dependency property.
        /// </summary>
        public DataTemplateSelector GroupHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(GroupHeaderTemplateSelectorProperty); }
            set { this.SetValue(GroupHeaderTemplateSelectorProperty, value); }
        }
        
        /// <summary>
        /// Gets the collection with all the custom commands registered with the <see cref="CommandService"/>. Custom commands have higher priority than the built-in (default) ones.
        /// </summary>
        public CommandCollection<RadDataForm> Commands
        {
            get
            {
                return this.commandService.UserCommands;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Windows.UI.Xaml.Controls.StyleSelector"/> used to choose Style to display each editor of the control. 
        /// This is a dependency property.
        /// </summary>
        public StyleSelector EditorStyleSelector
        {
            get { return (StyleSelector)GetValue(EditorStyleSelectorProperty); }
            set { this.SetValue(EditorStyleSelectorProperty, value); }
        }

        /// <summary>
        /// Gets the <see cref="CommandService"/> instance that manages the commanding behavior of this instance.
        /// </summary>
        public CommandService CommandService
        {
            get
            {
                return this.commandService;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CommitMode"/> of the control.
        /// </summary>
        public CommitMode CommitMode
        {
            get { return (CommitMode)GetValue(CommitModeProperty); }
            set { this.SetValue(CommitModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Gets the <see cref="TransactionService"/> used by the <see cref="RadDataForm"/>.
        /// </summary>
        public TransactionService TransactionService
        {
            get
            {
                return this.transactionService;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PropertyIteratorMode"/> used by the <see cref="RadDataForm"/>.
        /// </summary>
        public PropertyIteratorMode PropertyIteratorMode
        {
            get
            {
                return this.iteratorMode;
            }
            set
            {
                this.SetValue(PropertyIteratorModeProperty, value);
            }
        }
        
        ITransactionService IDataFormView.TransactionService
        {
            get { return this.TransactionService; }
        }

        bool IDataFormView.IsReadOnly
        {
            get
            {
                return this.IsReadOnly;
            }
        }

        internal DataFormModel Model { get; set; }

        internal Entity Entity
        {
            get
            {
                return this.Model.Entity;
            }
        }

        /// <summary>
        /// Method that returns the current root panel of the control.
        /// </summary>
        public Panel GetRootPanel()
        {
            return this.RootPanel;
        }

        /// <inheritdoc/>
        public void RegisterTypeEditor(Type propertyType, Type editorType)
        {
            this.Model.EditorFactory.RegisterTypeEditor(propertyType, editorType);
        }

        /// <inheritdoc/>
        public void RegisterPropertyEditor(string propertyName, Type editorType)
        {
            this.Model.EditorFactory.RegisterPropertyEditor(propertyName, editorType);
        }

        /// <inheritdoc/>
        public void RegisterTypeView(Type propertyType, Type viewType)
        {
            this.Model.EditorFactory.RegisterTypeView(propertyType, viewType);
        }

        /// <inheritdoc/>
        public void RegisterPropertyView(string propertyName, Type viewType)
        {
            this.Model.EditorFactory.RegisterPropertyView(propertyName, viewType);
        }

        /// <inheritdoc/>
        public void UnRegisterTypeEditor(Type propertyType = null)
        {
            this.Model.EditorFactory.UnRegisterTypeEditor(propertyType);
        }

        /// <inheritdoc/>
        public void UnRegisterPropertyEditor(string propertyName = null)
        {
            this.Model.EditorFactory.UnRegisterPropertyEditor(propertyName);
        }

        /// <inheritdoc/>
        public void UnRegisterTypeView(Type propertyType = null)
        {
            this.Model.EditorFactory.UnRegisterTypeView(propertyType);
        }

        /// <inheritdoc/>
        public void UnRegisterPropertyView(string propertyName = null)
        {
            this.Model.EditorFactory.UnRegisterPropertyView(propertyName);
        }

        /// <summary>
        /// Method used for adding an editor to the root panel of the control.
        /// </summary>
        public void AddEditor(object element)
        {
            if (this.IsTemplateApplied)
            {
                this.RootPanel.Children.Add(element as UIElement);
            }
        }

        /// <summary>
        /// Clears the editors of the control.
        /// </summary>
        public void ClearEditors()
        {
            if (this.IsTemplateApplied)
            {
                this.RootPanel.Children.Clear();
            }
        }

        /// <inheritdoc/>
        public void RefreshFormLayout()
        {
            this.Model.RefreshLayout();
        }

        void IDataFormView.PrepareEditor(object editor, object groupVisual)
        {
            var editorElement = editor as EntityPropertyControl;

            ((IDataFormView)this).PrepareEditor(editorElement, editorElement.Property, groupVisual);
        }

        void IDataFormView.PrepareEditor(object editor, EntityProperty entityProperty, object groupVisual)
        {
            var editorElement = editor as EntityPropertyControl;

            Panel parentPanel = groupVisual != null ? ((groupVisual as RadExpanderControl).ExpandableContent as Panel) : this.RootPanel;

            editorElement.DataContext = entityProperty;

            if (this.EditorStyleSelector != null)
            {
                editorElement.View.Style = this.EditorStyleSelector.SelectStyle(entityProperty, editorElement);
            }

            ((IDataFormView)this).SubscribeToEditorEvents(editorElement, entityProperty);

            editorElement.Property = entityProperty;

            this.LayoutDefinition.SetEditorArrangeMetadata(editorElement, entityProperty, parentPanel);

            this.LayoutDefinition.SetEditorElementsArrangeMetadata(editorElement, entityProperty);
        }

        void IDataFormView.PrepareGroupEditor(object editor, string groupKey)
        {
            var groupHeader = editor as ContentControl;

            groupHeader.Content = groupKey;

            if (this.GroupHeaderTemplateSelector != null)
            {
                groupHeader.ContentTemplate = this.GroupHeaderTemplateSelector.SelectTemplate(groupKey, groupHeader);
            }

            this.LayoutDefinition.SetGroupArrangeMetadata(groupHeader, groupKey);
        }

        void IDataFormView.SubscribeToEditorEvents(object editor, EntityProperty property)
        {
            var editorContent = editor as EntityPropertyControl;

            if (property != null)
            {
                property.PropertyChanged += this.EditorPropertyChanged;
            }
            if (editorContent != null)
            {
                editorContent.LostFocus += this.EditorLostFocus;
            }
        }

        void IDataFormView.UnSubscribeFromEditorEvents(object editor, EntityProperty property)
        {
            var editorContent = editor as EntityPropertyControl;

            if (editorContent != null)
            {
                editorContent.LostFocus -= this.EditorLostFocus;
            }
            if (property != null)
            {
                property.PropertyChanged -= this.EditorPropertyChanged;
            }
        }

        object IDataFormView.CreateGroupContainer(string groupKey)
        {
            RadExpanderControl groupHeader = new RadExpanderControl();
            groupHeader.UseSystemFocusVisuals = true;
            groupHeader.IsExpanded = true;
            groupHeader.ExpandableContent = this.LayoutDefinition.CreateGroupLayoutPanel(groupKey);

            return groupHeader;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Model.OnItemChanged(this.Item);
        }

        /// <inheritdoc/>
        protected override bool ApplyTemplateCore()
        {
            this.childrensPanelPresenter = this.GetTemplatePartField<ContentControl>("PART_ChildrensPanelPresenter");
            bool applied = this.childrensPanelPresenter != null;

            this.RootPanel = this.LayoutDefinition.CreateDataFormPanel();

            if (this.RootPanel != null)
            {
                this.childrensPanelPresenter.Content = this.RootPanel;
            }

            return applied && this.RootPanel != null;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            if (this.childrensPanelPresenter != null)
            {
                this.childrensPanelPresenter.Content = null;
            }

            base.UnapplyTemplateCore();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadDataFormAutomationPeer(this);
        }
        
        private static void OnLayoutDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var form = d as RadDataForm;

            if (form.IsTemplateApplied)
            {
                var children = form.RootPanel.Children.ToArray();
                form.RootPanel.Children.Clear();

                form.RootPanel = form.LayoutDefinition.CreateDataFormPanel();
                foreach (var item in children)
                {
                    form.RootPanel.Children.Add(item);
                }

                form.childrensPanelPresenter.Content = form.RootPanel;
            }
        }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataForm form = d as RadDataForm;
            if (form.IsTemplateApplied)
            {
                form.Model.OnItemChanged(e.NewValue);
            }
        }

        private static void OnEntityProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadDataForm).Model.OnEntityProviderChanged(e.NewValue as EntityProvider);
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataForm owner = d as RadDataForm;
            owner.Model.OnIsReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        private static void OnEditorFactoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var df = d as RadDataForm;

            if (e.NewValue != null)
            {
                (e.NewValue as EditorFactory).RestrictEditableControls = df.IsReadOnly;
            }

            df.Model.OnEditorFactoryChanged(df.EditorFactory);
        }

        private static void OnCommitModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataForm owner = d as RadDataForm;
            owner.Model.OnCommitModeChanged((CommitMode)e.NewValue, (CommitMode)e.OldValue);
        }

        private static void OnPropertyIteratorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataForm form = d as RadDataForm;
            form.iteratorMode = (PropertyIteratorMode)e.NewValue;
        }

        private void EditorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PropertyValue")
            {
                if (this.ValidationMode == Data.ValidationMode.Immediate)
                {
                    this.CommandService.ExecuteCommand(CommandId.Validate, sender as EntityProperty);
                }
                if (this.CommitMode == Data.CommitMode.Immediate)
                {
                    this.CommandService.ExecuteCommand(CommandId.Commit, sender as EntityProperty);
                }
            }
        }

        private void EditorLostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var editor = sender as EntityPropertyControl;
            var entityProperty = editor.Property;
            if (this.ValidationMode == Data.ValidationMode.OnLostFocus)
            {
                this.CommandService.ExecuteCommand(CommandId.Validate, entityProperty);
            }

            if (this.CommitMode == CommitMode.OnLostFocus)
            {
                this.CommandService.ExecuteCommand(CommandId.Commit, entityProperty);
            }
        }
    }
}
