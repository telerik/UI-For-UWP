using System;
using Windows.System;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal static class KeyboardHelper
    {
        public static bool IsModifierKeyDown(VirtualKey key)
        {
            var window = CoreWindow.GetForCurrentThread();
            if (window == null)
            {
                return false;
            }

            var state = window.GetKeyState(key);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }
    }
}
