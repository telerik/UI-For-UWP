using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public class DataGridFormEditor : RadControl, IGridExternalEditor
    {
        private RadDataForm dataForm;

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(nameof(Item), typeof(object), typeof(DataGridFormEditor), new PropertyMetadata(null));

        public static readonly DependencyProperty DataFormStyleProperty =
            DependencyProperty.Register(nameof(DataFormStyle), typeof(Style), typeof(DataGridFormEditor), new PropertyMetadata(null));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(ExternalEditorPosition), typeof(DataGridFormEditor), new PropertyMetadata(ExternalEditorPosition.Right, OnPositionChanged));

        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(DataGridFormEditor), new PropertyMetadata(null));

        public static readonly DependencyProperty CancelCommandProperty =
           DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(DataGridFormEditor), new PropertyMetadata(null));

        public static readonly DependencyProperty SaveCommandProperty =
          DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(DataGridFormEditor), new PropertyMetadata(null));

        public DataGridFormEditor()
        {
            this.SaveCommand = new ExternalEditorActionCommand(this, Commands.ExternalEditorCommandId.Save);
            this.CancelCommand = new ExternalEditorActionCommand(this, Commands.ExternalEditorCommandId.Cancel);
            this.DefaultStyleKey = typeof(DataGridFormEditor);
        }

        internal RadDataGrid Owner { get; set; }

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        public ICommand SaveCommand
        {
            get { return (ICommand)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        public object Item
        {
            get { return (object)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public Style DataFormStyle
        {
            get { return (Style)GetValue(DataFormStyleProperty); }
            set { SetValue(DataFormStyleProperty, value); }
        }

        public ExternalEditorPosition Position
        {
            get { return (ExternalEditorPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }


        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.dataForm = this.GetTemplatePartField<RadDataForm>("PART_DataForm");
            applied = this.dataForm != null;

            return applied;
        }

        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.dataForm.EntityProvider = new DataGridFormEntityProvider(this.Owner.Columns);

            this.RegisterCustomEditors();

            this.UpdateVisualState(false);
        }

        public event EventHandler EditCancelled;

        public event EventHandler EditCommitted;

        public void BeginEdit(object item, RadDataGrid owner)
        {
            this.Owner = owner;
            this.Item = item;
        }

        public void CancelEdit()
        {
            this.Item = null;

            var handler = this.EditCancelled;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

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

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = d as DataGridFormEditor;
            if(editor != null)
            {
                if(editor.Owner != null)
                {
                    var flyoutId = (ExternalEditorPosition)e.OldValue == ExternalEditorPosition.Left ? DataGridFlyoutId.EditorLeft : DataGridFlyoutId.EditorRight;
                    editor.Owner.ContentFlyout.Hide(flyoutId);
                }
                editor.UpdateVisualState(false);
            }
        }

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

            return "";
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
