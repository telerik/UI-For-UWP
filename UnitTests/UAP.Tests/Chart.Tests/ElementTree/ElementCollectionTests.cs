using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Chart.Tests
{
    [TestClass]
    public class ElementCollectionTests
    {
        private TestChartElement chartElement;
        private Node node11;
        private Node node12;
        private Node node21;
        private Node node22;

        [TestInitialize]
        public void TestInitialize()
        {
            this.chartElement = new TestChartElement();
            this.node11 = new TestChartNode();
            this.node12 = new TestChartNode();
            this.node21 = new TestChartNode();
            this.node22 = new TestChartNode();
        }

        [TestMethod]
        public void TestElementCollectionAdd()
        {
            this.chartElement.ElementCollection1.Add(this.node11);

            Assert.IsTrue(this.chartElement.ElementCollection1.Count == 1);
            Assert.AreEqual(this.node11, this.chartElement.ElementCollection1[0]);

            Assert.IsTrue(this.chartElement.ElementCollection2.Count == 0);

            Assert.IsTrue(this.chartElement.children.Count == 1);
            Assert.AreEqual(this.node11, this.chartElement.children[0]);

            this.chartElement.ElementCollection2.Add(this.node21);

            Assert.IsTrue(this.chartElement.ElementCollection1.Count == 1);
            Assert.AreEqual(this.node11, this.chartElement.ElementCollection1[0]);

            Assert.IsTrue(this.chartElement.ElementCollection2.Count == 1);
            Assert.AreEqual(this.node21, this.chartElement.ElementCollection2[0]);

            Assert.IsTrue(this.chartElement.children.Count == 2);
            Assert.AreEqual(this.node11, this.chartElement.children[0]);
            Assert.AreEqual(this.node21, this.chartElement.children[1]);
        }

        [TestMethod]
        public void TestElementCollectionInsert()
        {
            this.chartElement.ElementCollection1.Add(this.node11);

            this.chartElement.ElementCollection1.Insert(0, this.node12);

            Assert.IsTrue(this.chartElement.ElementCollection1.Count == 2);
            Assert.AreEqual(this.node12, this.chartElement.ElementCollection1[0]);

            Assert.IsTrue(this.chartElement.children.Count == 2);
            Assert.AreEqual(this.node11, this.chartElement.children[0]);
            Assert.AreEqual(this.node12, this.chartElement.children[1]);
        }

        [TestMethod]
        public void TestElementCollectionRemove()
        {
            this.SetupElementCollections();

            this.chartElement.ElementCollection1.RemoveAt(0);

            Assert.IsTrue(this.chartElement.ElementCollection1.Count == 1);
            Assert.AreEqual(this.node12, this.chartElement.ElementCollection1[0]);

            Assert.IsTrue(this.chartElement.ElementCollection2.Count == 2);
            Assert.AreEqual(this.node21, this.chartElement.ElementCollection2[0]);
            Assert.AreEqual(this.node22, this.chartElement.ElementCollection2[1]);

            Assert.IsTrue(this.chartElement.children.Count == 3);
            Assert.AreEqual(this.node12, this.chartElement.children[0]);
            Assert.AreEqual(this.node21, this.chartElement.children[1]);
            Assert.AreEqual(this.node22, this.chartElement.children[2]);

            this.chartElement.ElementCollection1.Remove(this.node12);

            Assert.IsTrue(this.chartElement.ElementCollection1.Count == 0);

            Assert.IsTrue(this.chartElement.ElementCollection2.Count == 2);
            Assert.AreEqual(this.node21, this.chartElement.ElementCollection2[0]);
            Assert.AreEqual(this.node22, this.chartElement.ElementCollection2[1]);

            Assert.IsTrue(this.chartElement.children.Count == 2);
            Assert.AreEqual(this.node21, this.chartElement.children[0]);
            Assert.AreEqual(this.node22, this.chartElement.children[1]);
        }

        [TestMethod]
        public void TestElementCollectionClear()
        {
            this.SetupElementCollections();

            this.chartElement.ElementCollection1.Clear();

            Assert.IsTrue(this.chartElement.ElementCollection1.Count == 0);

            Assert.IsTrue(this.chartElement.ElementCollection2.Count == 2);
            Assert.AreEqual(this.node21, this.chartElement.ElementCollection2[0]);
            Assert.AreEqual(this.node22, this.chartElement.ElementCollection2[1]);

            Assert.IsTrue(this.chartElement.children.Count == 2);
            Assert.AreEqual(this.node21, this.chartElement.children[0]);
            Assert.AreEqual(this.node22, this.chartElement.ElementCollection2[1]);
        }

        private void SetupElementCollections()
        {
            this.chartElement.ElementCollection1.Add(this.node11);
            this.chartElement.ElementCollection1.Add(this.node12);

            this.chartElement.ElementCollection2.Add(this.node21);
            this.chartElement.ElementCollection2.Add(this.node22);
        }

        [TestMethod]
        public void TestElementCollectionClearAll()
        {
            this.chartElement.ElementCollection1.Clear();
            this.chartElement.ElementCollection2.Clear();

            Assert.IsTrue(this.chartElement.ElementCollection1.Count == 0);
            Assert.IsTrue(this.chartElement.ElementCollection2.Count == 0);
            Assert.IsTrue(this.chartElement.children.Count == 0);
        }
    }
}
