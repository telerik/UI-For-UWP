using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    public class CurrentSelectionChangedEventArgs : EventArgs
    {
        public DateTime NewSelection { get; set; }
    }
}
