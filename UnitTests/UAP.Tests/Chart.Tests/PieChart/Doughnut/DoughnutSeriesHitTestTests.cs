using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Charting;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart.Tests.PieChart.Doughnut
{
    [TestClass]
    public class DoughnutSeriesHitTestTests
    {
        [TestMethod]
        public void HitTestingDoughnutPoints_WhenRectangleIsInsideBounds_ShouldReturnTrue()
        {
            DoughnutDataPoint point = new DoughnutDataPoint
            {
                sweepAngle = 90,
                startAngle = 5,
                InnerRadius = 50,
                CenterPoint = new Point(100, 100),
                Radius = 100
            };

            var touchRect = new Rect(150, 150, 1, 1);


            Assert.IsTrue(point.ContainsRect(touchRect));
        }

        [TestMethod]
        public void HitTestingDoughnutPoints_WhenRectangleIsOutsideBounds_ShouldReturnFalse()
        {
            DoughnutDataPoint point = new DoughnutDataPoint
            {
                sweepAngle = 90,
                startAngle = 5,
                InnerRadius = 50,
                CenterPoint = new Point(100, 100),
                Radius = 100
            };

            var touchRect = new Rect(50, 150, 1, 1);


            Assert.IsTrue(!point.ContainsRect(touchRect));
        }
    }
}
