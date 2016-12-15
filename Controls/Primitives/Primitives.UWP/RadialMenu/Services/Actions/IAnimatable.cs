using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal interface IAnimatable
    {
        event EventHandler<AnimationCompleteEventArgs> AnimationComplete;

        void Show(Action completeAction);

        void Hide(Action completeAction);

        void NavigateFrom(Action completeAction);

        void NavigateTo(Action completeAction);
    }
}
