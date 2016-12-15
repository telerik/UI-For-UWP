using System;
using System.Linq;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class ChartAnnotationModel : Element
    {
        internal RadRect originalLayoutSlot;

        public ChartAnnotationModel()
        {
            this.TrackPropertyChanged = true;
        }

        public abstract bool IsUpdated { get; }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            RadRect finalRect = base.ArrangeOverride(rect);

            this.Update();
            
            if (this.IsUpdated)
            {
                finalRect = this.ArrangeCore(rect);
            }

            return finalRect;
        }

        internal abstract void ResetState();

        protected static bool TryCreatePlotInfo(AxisModel axis, object value, out AxisPlotInfo plotInfo)
        {
            if (axis == null || value == null || !axis.isUpdated)
            {
                plotInfo = null;
                return false;
            }

            plotInfo = axis.CreatePlotInfo(value);

            return plotInfo != null;
        }

        protected void Update()
        {
            if (!this.IsUpdated)
            {
                this.UpdateCore();
            }
        }

        protected abstract void UpdateCore();

        protected abstract RadRect ArrangeCore(RadRect rect);
    }
}