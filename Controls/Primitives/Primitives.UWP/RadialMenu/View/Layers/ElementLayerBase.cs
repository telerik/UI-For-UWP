using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal abstract class ElementLayerBase<T> : LayerBase where T : RadialSegment
    {
        public abstract void ClearVisual(T segment);

        public abstract void ShowVisual(T segment, double startAngle);

        public abstract void UpdateVisual(T segment, double startAngle);
    }
}
