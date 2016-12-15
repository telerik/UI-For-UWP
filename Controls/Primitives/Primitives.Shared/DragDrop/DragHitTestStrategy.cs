using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives.DragDrop
{
    internal class DragHitTestStrategy
    {
        private GeneralTransform transformationToGlobalCoordinates;

        private UIElement rootElement;

        public DragHitTestStrategy(UIElement rootElement)
        {
            var appRootVisual = Window.Current.Content as UIElement;
            if (appRootVisual != null)
            {
                this.transformationToGlobalCoordinates = rootElement.TransformToVisual(appRootVisual);
            }

            this.rootElement = rootElement;
        }

        protected virtual UIElement RootElement
        {
            get
            {
                return this.rootElement;
            }
        }

        internal IDragDropElement GetTarget(Rect hitTestBounds, Point restrictedPointerPosition)
        {
            if (this.transformationToGlobalCoordinates != null)
            {
                var transformedDragPositionRect = this.transformationToGlobalCoordinates.TransformBounds(hitTestBounds);
                var transformedDragPoint = this.transformationToGlobalCoordinates.TransformPoint(restrictedPointerPosition);
                return this.HitTest(transformedDragPositionRect, transformedDragPoint).FirstOrDefault();
            }

            return null;
        }

        protected virtual IEnumerable<IDragDropElement> HitTest(Rect globalHitTestBounds, Point restrictedPointerPosition)
        {
            return VisualTreeHelper.FindElementsInHostCoordinates(globalHitTestBounds, this.RootElement).OfType<IDragDropElement>().Where(c => !c.SkipHitTest);
        }
    }
}