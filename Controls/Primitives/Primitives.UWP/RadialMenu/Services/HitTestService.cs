using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class HitTestService
    {
        private WeakReferenceList<IHitTestArea> registeredAreas;
        private RadRadialMenu owner;

        public HitTestService(RadRadialMenu owner)
        {
            this.registeredAreas = new WeakReferenceList<IHitTestArea>();
            this.owner = owner;
        }

        /// <summary>
        /// Gets the hit test registered areas. Exposed for testing purposes.
        /// </summary>
        internal WeakReferenceList<IHitTestArea> RegisteredAreas
        {
            get
            {
                return this.registeredAreas;
            }
        }

        public void RegisterArea(IHitTestArea area)
        {
            this.registeredAreas.Add(area);
        }

        public void UnregisterArea(IHitTestArea area)
        {
            this.registeredAreas.Remove(area);
        }

        public IEnumerable<RadialSegment> HitTest(Point point)
        {
            foreach (var area in this.registeredAreas)
            {
                if (area.HitTestVisible)
                {
                    var polarPointTuple = RadMath.ToPolarCoordinates(new RadPoint(point.X, point.Y), new RadPoint(this.owner.ActualWidth / 2, this.owner.ActualHeight / 2), true);

                    var segment = area.HitTest(new RadPolarPoint(polarPointTuple.Item1, polarPointTuple.Item2));

                    if (segment != null)
                    {
                        yield return segment;
                    }
                }
            }

            yield break;
        }
    }
}
