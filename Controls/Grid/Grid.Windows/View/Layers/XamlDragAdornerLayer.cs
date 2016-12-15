using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class XamlDragAdornerLayer : SharedUILayer
    {
        private DataGridDragSurface dragSurface;

        public XamlDragAdornerLayer()
        {
            this.dragSurface = new DataGridDragSurface(this);
        }

        internal IDragSurface DragSurface
        {
            get
            {
                return this.dragSurface;
            }
        }
    }
}
