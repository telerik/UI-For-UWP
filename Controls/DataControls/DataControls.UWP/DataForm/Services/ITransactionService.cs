using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    internal interface ITransactionService
    {
        void ValidateProperty(EntityProperty property);

        void CommitPropertyCore(EntityProperty property);

        void ErrorsChanged(object sender, string propertyName);
    }
}
