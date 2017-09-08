using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Data.DataForm.Commands;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal interface IDataFormView
    {
        PropertyIteratorMode PropertyIteratorMode { get; }
        CommitMode CommitMode { get; set; }
        CommandService CommandService { get; }
        ITransactionService TransactionService { get; }

        bool IsReadOnly { get; }

        void AddEditor(object element);

        void ClearEditors();

        object CreateGroupContainer(string groupKey);

        void PrepareEditor(object editor, object groupVisual);

        void PrepareEditor(object editor, EntityProperty property, object groupVisual);

        void PrepareGroupEditor(object editor, string groupKey);

        void SubscribeToEditorEvents(object editor, EntityProperty property);

        void UnSubscribeFromEditorEvents(object editor, EntityProperty property);
    }
}
