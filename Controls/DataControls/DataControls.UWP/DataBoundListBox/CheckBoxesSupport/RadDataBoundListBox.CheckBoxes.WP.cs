namespace Telerik.UI.Xaml.Controls.Data
{
#if WINDOWS_PHONE_APP
    public partial class RadDataBoundListBox
    {
        private bool backKeyPressHooked = false;

        internal virtual void OnBackKeyPressed(Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.isCheckModeActive)
            {
                this.IsCheckModeActive = false;
                e.Handled = true;
            }
        }

        partial void HookRootVisualBackKeyPress()
        {
            if (!this.checkModeDeactivatedOnBackButton)
            {
                return;
            }

            if (this.backKeyPressHooked)
            {
                return;
            }

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += this.OnPageBackKeyPress;
            this.backKeyPressHooked = true;
        }

        partial void UnhookRootVisualBackKeyPress()
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= this.OnPageBackKeyPress;
            this.backKeyPressHooked = false;
        }

        private void OnPageBackKeyPress(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            this.OnBackKeyPressed(e);
        }
    }
#endif
}
