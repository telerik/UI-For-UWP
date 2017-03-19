using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Data.DataForm.Commands;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using System.Linq;
using Windows.UI.Xaml.Automation.Peers;
using Telerik.UI.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Data
{
    [TemplatePart(Name = "PART_ChildrensPanelPresenter", Type = typeof(ContentControl))]
    public class RadDataForm : RadControl, IDataFormView
    {
        internal Panel RootPanel;

        public DataFormLayoutDefinition LayoutDefinition
        {
            get { return (DataFormLayoutDefinition)GetValue(LayoutDefinitionProperty); }
            set { SetValue(LayoutDefinitionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayoutDefinition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayoutDefinitionProperty =
            DependencyProperty.Register(nameof(LayoutDefinition), typeof(DataFormLayoutDefinition), typeof(RadDataForm), new PropertyMetadata(null, OnLayoutDefinitionChanged));

        private static void OnLayoutDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var form = d as RadDataForm;

            if (form.IsTemplateApplied)
            {
                var children = form.RootPanel.Children.ToList();
                form.RootPanel.Children.Clear();

                form.RootPanel = form.LayoutDefinition.CreateDataFormPanel();
                foreach (var item in children)
                {
                    form.RootPanel.Children.Add(item);
                }

                form.childrensPanelPresenter.Content = form.RootPanel;
            }

        }

        public ValidationMode ValidationMode
        {
            get { return (ValidationMode)GetValue(ValidationModeProperty); }
            set { SetValue(ValidationModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValidationMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValidationModeProperty =
            DependencyProperty.Register(nameof(ValidationMode), typeof(ValidationMode), typeof(RadDataForm), new PropertyMetadata(ValidationMode.OnCommit));

        // Using a DependencyProperty as the backing store for PropertyIteratorMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyIteratorModeProperty =
            DependencyProperty.Register(nameof(PropertyIteratorMode), typeof(PropertyIteratorMode), typeof(RadDataForm), new PropertyMetadata(PropertyIteratorMode.All, OnPropertyIteratorModeChanged));

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item), typeof(object), typeof(RadDataForm), new PropertyMetadata(null, OnItemChanged));



        public EditorFactory EditorFactory
        {
            get { return (EditorFactory)GetValue(EditorFactoryProperty); }
            set { SetValue(EditorFactoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditorFactory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorFactoryProperty =
            DependencyProperty.Register(nameof(EditorFactory), typeof(EditorFactory), typeof(RadDataForm), new PropertyMetadata(null, OnEditorFactoryChanged));

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RadDataForm), new PropertyMetadata(false, OnIsReadOnlyChanged));

        public DataTemplateSelector GroupHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(GroupHeaderTemplateSelectorProperty); }
            set { SetValue(GroupHeaderTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupHeaderTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupHeaderTemplateSelectorProperty =
            DependencyProperty.Register(nameof(GroupHeaderTemplateSelector), typeof(DataTemplateSelector), typeof(RadDataForm), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for EditorStyleSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditorStyleSelectorProperty =
            DependencyProperty.Register(nameof(EditorStyleSelector), typeof(StyleSelector), typeof(RadDataForm), new PropertyMetadata(null));

        public StyleSelector EditorStyleSelector
        {
            get { return (StyleSelector)GetValue(EditorStyleSelectorProperty); }
            set { SetValue(EditorStyleSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValidatorProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EntityProviderProperty =
            DependencyProperty.Register(nameof(EntityProvider), typeof(EntityProvider), typeof(RadDataForm), new PropertyMetadata(null, OnEntityProviderChanged));

        private static void OnEntityProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadDataForm).Model.OnEntityProviderChanged(e.NewValue as EntityProvider);
        }

        public EntityProvider EntityProvider
        {
            get { return (EntityProvider)GetValue(EntityProviderProperty); }
            set { SetValue(EntityProviderProperty, value); }
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

        private CommandService commandService;
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

        public CommitMode CommitMode
        {
            get { return (CommitMode)GetValue(CommitModeProperty); }
            set { SetValue(CommitModeProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
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

        // Using a DependencyProperty as the backing store for ValidationMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommitModeProperty =
            DependencyProperty.Register(nameof(CommitMode), typeof(CommitMode), typeof(RadDataForm), new PropertyMetadata(CommitMode.Immediate, OnCommitModeChanged));

        private static void OnCommitModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataForm owner = d as RadDataForm;
            owner.Model.OnCommitModeChanged((CommitMode)e.NewValue, (CommitMode)e.OldValue);
        }

        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        private PropertyIteratorMode iteratorMode;
        internal DataFormModel Model { get; set; }
        private TransactionService transactionService;

        public RadDataForm()
        {
            this.DefaultStyleKey = (typeof(RadDataForm));
            this.Model = new DataFormModel(this);
            this.transactionService = new TransactionService(this);
            this.commandService = new CommandService(this);
        }

        public TransactionService TransactionService
        {
            get
            {
                return this.transactionService;
            }
        }

        public PropertyIteratorMode PropertyIteratorMode
        {
            get
            {
                return this.iteratorMode;
            }
            set
            {
                SetValue(PropertyIteratorModeProperty, value);
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

        private static void OnPropertyIteratorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadDataForm form = d as RadDataForm;
            form.iteratorMode = (PropertyIteratorMode)e.NewValue;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Model.OnItemChanged(this.Item);
        }
        internal ContentControl childrensPanelPresenter;
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

        protected override void UnapplyTemplateCore()
        {
            if (this.childrensPanelPresenter != null)
            {
                this.childrensPanelPresenter.Content = null;
            }

            base.UnapplyTemplateCore();
        }

        public Panel GetRootPanel()
        {
            return this.RootPanel;
        }

        public void RegisterTypeEditor(Type propertyType, Type editorType)
        {
            this.Model.EditorFactory.RegisterTypeEditor(propertyType, editorType);
        }

        public void RegisterPropertyEditor(string propertyName, Type editorType)
        {
            this.Model.EditorFactory.RegisterPropertyEditor(propertyName, editorType);
        }

        public void RegisterTypeView(Type propertyType, Type viewType)
        {
            this.Model.EditorFactory.RegisterTypeView(propertyType, viewType);
        }

        public void RegisterPropertyView(string propertyName, Type viewType)
        {
            this.Model.EditorFactory.RegisterPropertyView(propertyName, viewType);
        }

        public void UnRegisterTypeEditor(Type propertyType = null)
        {
            this.Model.EditorFactory.UnRegisterTypeEditor(propertyType);
        }

        public void UnRegisterPropertyEditor(string propertyName = null)
        {
            this.Model.EditorFactory.UnRegisterPropertyEditor(propertyName);
        }

        public void UnRegisterTypeView(Type propertyType = null)
        {
            this.Model.EditorFactory.UnRegisterTypeView(propertyType);
        }

        public void UnRegisterPropertyView(string propertyName = null)
        {
            this.Model.EditorFactory.UnRegisterPropertyView(propertyName);
        }

        public void AddEditor(object element)
        {
            if (this.IsTemplateApplied)
            {
                this.RootPanel.Children.Add(element as UIElement);
            }
        }

        public void ClearEditors()
        {
            if (this.IsTemplateApplied)
            {
                this.RootPanel.Children.Clear();
            }
        }

        public void RefreshFormLayout()
        {
            this.Model.RefreshLayout();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadDataFormAutomationPeer(this);
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
                property.PropertyChanged += EditorPropertyChanged;
            }
            if (editorContent != null)
            {
                editorContent.LostFocus += EditorLostFocus;
            }
        }

        void IDataFormView.UnSubscribeFromEditorEvents(object editor, EntityProperty property)
        {
            var editorContent = editor as EntityPropertyControl;

            if (editorContent != null)
            {
                editorContent.LostFocus -= EditorLostFocus;
            }
            if (property != null)
            {
                property.PropertyChanged -= EditorPropertyChanged;
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

        internal Entity Entity
        {
            get
            {
                return this.Model.Entity;
            }
        }
    }
}
