using System;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a DataGridFormEditor control.
    /// </summary>
    public class DataGridFormEditor : RadControl, IGridExternalEditor
    {
        /// <summary>
        /// Identifies the <see cref="Item"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item), typeof(object), typeof(DataGridFormEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DataFormStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty DataFormStyleProperty =
            DependencyProperty.Register(nameof(DataFormStyle), typeof(Style), typeof(DataGridFormEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(ExternalEditorPosition), typeof(DataGridFormEditor), new PropertyMetadata(ExternalEditorPosition.Right, OnPositionChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderStyle"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(DataGridFormEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CancelCommand"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty CancelCommandProperty =
           DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(DataGridFormEditor), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SaveCommand"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SaveCommandProperty =
          DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(DataGridFormEditor), new PropertyMetadata(null));

        private RadDataForm dataForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFormEditor"/> class.
        /// </summary>
        public DataGridFormEditor()
        {
            this.SaveCommand = new ExternalEditorActionCommand(this, Commands.ExternalEditorCommandId.Save);
            this.CancelCommand = new ExternalEditorActionCommand(this, Commands.ExternalEditorCommandId.Cancel);
            this.DefaultStyleKey = typeof(DataGridFormEditor);
        }

        /// <inheritdoc/>
        public event EventHandler EditCancelled;

        /// <inheritdoc/>
        public event EventHandler EditCommitted;

        /// <summary>
        /// Gets or sets the <see cref="CancelCommand"/> for the <see cref="DataGridFormEditor"/>.
        /// </summary>
        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { this.SetValue(CancelCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="SaveCommand"/> for the <see cref="DataGridFormEditor"/>.
        /// </summary>
        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { this.SetValue(SaveCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Style of the Header for the <see cref="DataGridFormEditor"/>.
        /// </summary>
        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { this.SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Item of the <see cref="DataGridFormEditor"/>.
        /// </summary>
        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { this.SetValue(ItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Style for th DataForm.
        /// </summary>
        public Style DataFormStyle
        {
            get { return (Style)GetValue(DataFormStyleProperty); }
            set { this.SetValue(DataFormStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the external position of the <see cref="DataGridFormEditor"/>.
        /// </summary>
        public ExternalEditorPosition Position
        {
            get { return (ExternalEditorPosition)GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        internal RadDataGrid Owner { get; set; }

        /// <inheritdoc/>
        public void BeginEdit(object item, RadDataGrid owner)
        {
            this.Owner = owner;
            this.Item = item;
        }

        /// <inheritdoc/>
        public void CancelEdit()
        {
            this.Item = null;

            var handler = this.EditCancelled;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc/>
        public void CommitEdit()
        {
            if (this.IsTemplateApplied)
            {
                this.dataForm.TransactionService.CommitAll();
            }

            var handler = this.EditCommitted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate"/> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.dataForm = this.GetTemplatePartField<RadDataForm>("PART_DataForm");
            applied = this.dataForm != null;

            return applied;
        }

        /// <inheritdoc/>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.dataForm.EntityProvider = new DataGridFormEntityProvider(this.Owner.Columns);

            this.RegisterCustomEditors();

            this.UpdateVisualState(false);
        }
        
        /// <inheritdoc/>
        protected override string ComposeVisualStateName()
        {
            if (this.Position == ExternalEditorPosition.Left)
            {
                return "RightBorder";
            }
            else if (this.Position == ExternalEditorPosition.Right)
            {
                return "LeftBorder";
            }

            return string.Empty;
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as DataGridFormEditor;
            if (editor != null)
            {
                if (editor.Owner != null)
                {
                    var flyoutId = (ExternalEditorPosition)e.OldValue == ExternalEditorPosition.Left ? DataGridFlyoutId.EditorLeft : DataGridFlyoutId.EditorRight;
                    editor.Owner.ContentFlyout.Hide(flyoutId);
                }
                editor.UpdateVisualState(false);
            }
        }

        private void RegisterCustomEditors()
        {
            var columns = this.Owner.Columns;
            if (columns != null)
            {
                foreach (var column in columns)
                {
                    var comboColumn = column as DataGridComboBoxColumn;
                    if (comboColumn != null)
                    {
                        this.dataForm.RegisterPropertyEditor(comboColumn.PropertyName, typeof(ComboBoxEditor));
                    }
                }
            }
        }
    }
}
