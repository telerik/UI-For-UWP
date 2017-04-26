using Telerik.Core;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal interface ISupportItemAnimation
    {
        RadAnimation ItemRemovedAnimation { get; }

        RadAnimation ItemAddedAnimation { get; }

        ItemAnimationMode ItemAnimationMode { get; }

        Panel AnimatingChildrenPanel { get; }
    }
}
