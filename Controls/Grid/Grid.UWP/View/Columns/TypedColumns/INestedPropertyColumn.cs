namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface INestedPropertyColumn
    {
        string DisplayMemberPath { get; set; }

        object GetDisplayValueForInstance(object instance);
    }
}