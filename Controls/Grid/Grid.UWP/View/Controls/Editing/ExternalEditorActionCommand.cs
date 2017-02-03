using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
        public class ExternalEditorActionCommand : ICommand
        {
            private ExternalEditorCommandId Id { get; set; }
            public IGridExternalEditor Owner { get; set; }

            public ExternalEditorActionCommand(IGridExternalEditor owner, ExternalEditorCommandId id)
            {
                this.Owner = owner;
                this.Id = id;
            }

            public bool CanExecute(object parameter)
            {
                return this.Owner != null;
            }

#pragma warning disable 0067
            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                if (this.Owner == null)
                {
                    return;
                }

                if (this.Id == ExternalEditorCommandId.Save)
                {
                    this.Owner.CommitEdit();
                }
                else if (this.Id == ExternalEditorCommandId.Cancel)
                {
                    this.Owner.CancelEdit();
                }
            }
        }
}
