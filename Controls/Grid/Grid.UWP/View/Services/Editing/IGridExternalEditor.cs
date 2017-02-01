using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public interface IGridExternalEditor
    {
        void BeginEdit(object item, RadDataGrid owner);
        void CancelEdit();
        void CommitEdit();
        event EventHandler EditCancelled;
        event EventHandler EditCommitted;
        ExternalEditorPosition Position { get; set; }
    }
}
