using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class AnimationCompleteEventArgs : EventArgs
    {
        public AnimationOperation Action { get; set; }
    }
}
