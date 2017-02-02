using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    public class GroupPositionToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var position = (GroupPanelPosition)value;
            
            if(position == GroupPanelPosition.Left)
            {
                return VerticalAlignment.Stretch;
            }
            if(position == GroupPanelPosition.Bottom)
            {
                return VerticalAlignment.Bottom;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
