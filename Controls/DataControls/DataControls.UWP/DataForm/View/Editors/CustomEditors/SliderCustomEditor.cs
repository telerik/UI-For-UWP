using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    public class SliderCustomEditor : CustomEditorBase<Slider>
    {
        public SliderCustomEditor()
        {
            this.DefaultStyleKey = typeof(SliderCustomEditor);
        }
    }
}
