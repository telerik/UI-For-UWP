using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    public class ToggleSwitchCustomEditor : CustomEditorBase<ToggleSwitch>
    {
        public ToggleSwitchCustomEditor()
        {
            this.DefaultStyleKey = typeof(ToggleSwitchCustomEditor);
        }
    }
}
