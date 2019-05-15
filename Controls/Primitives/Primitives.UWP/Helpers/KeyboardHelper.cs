using Windows.System;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal static class KeyboardHelper
    {
        public static bool IsModifierKeyDown(VirtualKey key)
        {
            var state = KeyboardHelper.GetKeyState(key);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        public static bool IsModifierKeyLocked(VirtualKey key)
        {
            var state = KeyboardHelper.GetKeyState(key);
            return (state & CoreVirtualKeyStates.Locked) == CoreVirtualKeyStates.Locked;
        }

        private static CoreVirtualKeyStates GetKeyState(VirtualKey key)
        {
            var window = CoreWindow.GetForCurrentThread();
            if (window == null)
            {
                return CoreVirtualKeyStates.None;
            }

            return window.GetKeyState(key);
        }
    }
}