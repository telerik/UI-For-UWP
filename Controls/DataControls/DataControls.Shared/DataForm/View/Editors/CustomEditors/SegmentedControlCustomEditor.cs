using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Input;

namespace Telerik.UI.Xaml.Controls.Data
{
    public class SegmentedCustomEditor : CustomEditorBase<RadSegmentedControl>
    {
        public SegmentedCustomEditor()
        {
            this.DefaultStyleKey = typeof(SegmentedCustomEditor);
        }
    }
}
