namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal interface IAnimated
    {
        bool IsAnimating { get; set; }

        object Container { get; set; }
    }
}
