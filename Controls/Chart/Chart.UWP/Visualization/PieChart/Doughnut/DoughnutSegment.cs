using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class DoughnutSegment : PieSegment
    {
        private Path path;

        public override Path Path
        {
            get
            {
                if (this.path == null)
                {
                    this.path = new Path();
                }

                return this.path;
            }
        }

        protected override void UpdateCore(PieDataPoint dataPoint, PieUpdateContext context)
        {
            var doughnutContext = context as DoughnutUpdateContext;

            if (doughnutContext != null)
            {
                var segmentData = DoughnutSegment.GetSegmentData(dataPoint, doughnutContext);
                var highlightSegmentData = DoughnutSegment.GetSegmentData(dataPoint, doughnutContext);
                highlightSegmentData.Radius2 = 0.9 * context.Radius;

                if (context.SweepDirection == SweepDirection.Clockwise)
                {
                    context.StartAngle += dataPoint.SweepAngle;
                }
                else
                {
                    context.StartAngle -= dataPoint.SweepAngle;
                }

                this.Path.Data = DoughnutSegmentRenderer.Render(dataPoint, segmentData);
                this.HighlightPath.Data = DoughnutSegmentRenderer.Render(dataPoint, highlightSegmentData);

                var doughnutPoint = dataPoint as DoughnutDataPoint;

                if (doughnutPoint != null)
                {
                    doughnutPoint.Radius = doughnutContext.Radius;
                    doughnutPoint.InnerRadius = doughnutContext.Radius * doughnutContext.InnerRadiusFactor;
                    doughnutPoint.CenterPoint = doughnutContext.Center;
                }
            }
            else
            {
                base.UpdateCore(dataPoint, context);
            }
        }

        private static DoughnutSegmentData GetSegmentData(PieDataPoint dataPoint, DoughnutUpdateContext context)
        {
            var centerPoint = GetCenterPointWithOffset(dataPoint, context);

            return new DoughnutSegmentData()
            {
                Center = new RadPoint(centerPoint.X, centerPoint.Y),
                Radius1 = context.Radius,
                Radius2 = context.InnerRadiusFactor * context.Radius,
                StartAngle = context.StartAngle,
                SweepAngle = dataPoint.SweepAngle,
                SweepDirection = context.SweepDirection
            };
        }
    }
}