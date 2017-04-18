using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Chart.Tests
{
    [TestClass]
    public class RadRectTests
    {
        [TestMethod]
        public void Test_InterceptsWith()
        {
            RadRect rect1 = new RadRect(10, 10, 20, 20);
            Assert.IsTrue(rect1.IntersectsWith(rect1));

            RadRect rect2 = new RadRect(15, 15, 20, 20);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(15, 5, 20, 20);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(15, 15, 10, 10);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(15, 5, 40, 40);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(5, 5, 40, 40);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(5, 15, 40, 40);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(15, 15, 10, 10);
            Assert.IsTrue(rect1.IntersectsWith(rect2));
            Assert.IsTrue(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(5, 5, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(15, 5, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(5, 15, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(5, 35, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(15, 35, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(35, 5, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(35, 15, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));

            rect2 = new RadRect(35, 35, 4, 4);
            Assert.IsFalse(rect1.IntersectsWith(rect2));
            Assert.IsFalse(rect2.IntersectsWith(rect1));
        } 
    }
}
