namespace Telerik.Data.Core
{
    internal interface IDataStatusListener
    {
        void OnDataStatusChanged(DataProviderStatus status);
    }
}