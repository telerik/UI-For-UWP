using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.Common
{
    internal interface IPullToRefreshListener
    {
        ScrollViewer ScrollViewer { get; }

        FrameworkElement CompressedChildToTranslate { get; }

        FrameworkElement MainElementToTranslate { get; }

        Orientation Orientation { get; }

        void OnStarted();

        void OnEnded();

        void OnRefreshRequested();

        void OnOffsetChanged(double offset);
    }
}
