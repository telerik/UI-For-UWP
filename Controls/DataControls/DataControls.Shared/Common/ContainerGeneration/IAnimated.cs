using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal interface IAnimated
    {
        bool IsAnimating { get; set; }

        object Container { get; set; }
    }
}
