using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Input.Tests.Calendar.Services
{
    [TestClass]
    public class DateRangeCollectionTests
    {
        private CalendarDateRangeCollection dateRangeCollection;

        [TestInitialize]
        public void TestInitialize()
        {
            this.dateRangeCollection = new CalendarDateRangeCollection(null);
        }

        [TestMethod]
        public void Test_DateRanges_NoRangeCollisions_AssertNoRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 10)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 20), new DateTime(2013, 1, 30)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1), new DateTime(2013, 2, 10)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(newRangesList.Count, this.dateRangeCollection.Count, "Date range merges occure");

            for (int i = 0; i < this.dateRangeCollection.Count; i++)
            {
                Assert.AreEqual(newRangesList[i].StartDate, this.dateRangeCollection[i].StartDate);
                Assert.AreEqual(newRangesList[i].EndDate, this.dateRangeCollection[i].EndDate);
            }
        }

        [TestMethod]
        public void Test_DateRanges_NoRangeCollisions_AssertNoRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 10, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 20, 13, 13, 13), new DateTime(2013, 1, 30, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1, 13, 13, 13), new DateTime(2013, 2, 10, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(newRangesList.Count, this.dateRangeCollection.Count, "Date range merges occure");

            for (int i = 0; i < this.dateRangeCollection.Count; i++)
            {
                Assert.AreEqual(newRangesList[i].StartDate, this.dateRangeCollection[i].StartDate);
                Assert.AreEqual(newRangesList[i].EndDate, this.dateRangeCollection[i].EndDate);
            }
        }

        [TestMethod]
        public void Test_DateRanges_NoRangeCollisions_ThenFollowedMerge_SingleDates_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 1)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 3), new DateTime(2013, 1, 3)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 5), new DateTime(2013, 1, 5)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }
            Assert.AreEqual<int>(newRangesList.Count, this.dateRangeCollection.Count, "Date range merges occure");

            this.dateRangeCollection.AddDateRange(new CalendarDateRange(new DateTime(2013, 1, 2), new DateTime(2013, 1, 2)));
            this.dateRangeCollection.AddDateRange(new CalendarDateRange(new DateTime(2013, 1, 4), new DateTime(2013, 1, 4)));

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count, "Date range merges occure");

            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection.FirstOrDefault().StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection.FirstOrDefault().EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_NoRangeCollisions_ThenFollowedMerge_SingleDates_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 1, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 3, 13, 13, 13), new DateTime(2013, 1, 3, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 5, 13, 13, 13), new DateTime(2013, 1, 5, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }
            Assert.AreEqual<int>(newRangesList.Count, this.dateRangeCollection.Count, "Date range merges occure");

            this.dateRangeCollection.AddDateRange(new CalendarDateRange(new DateTime(2013, 1, 2, 13, 13, 13), new DateTime(2013, 1, 2, 15, 15, 15)));
            this.dateRangeCollection.AddDateRange(new CalendarDateRange(new DateTime(2013, 1, 4, 13, 13, 13), new DateTime(2013, 1, 4, 15, 15, 15)));

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count, "Date range merges occure");

            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection.FirstOrDefault().StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection.FirstOrDefault().EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_RepeatedRangeAdded_AssertSameRangeAdded()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 10)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 10)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 10)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList[0].StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList[0].EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_RepeatedRangeAdded_AssertSameRangeAdded_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 10, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 10, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 10, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList[0].StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList[0].EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_SameIndexPositon_GreaterEndDate_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 10)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 20)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 30)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_SameIndexPositon_GreaterEndDate_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 10, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 20, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 30, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_CoverEntireRanges_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 3, 1), new DateTime(2013, 4, 1)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1), new DateTime(2013, 5, 10)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 6, 10)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList[2].StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList[2].EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_CoverEntireRanges_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 3, 1, 13, 13, 13), new DateTime(2013, 4, 1, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1, 13, 13, 13), new DateTime(2013, 5, 10, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 6, 10, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList[2].StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList[2].EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_IncludesEntireRange_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 6, 10)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1), new DateTime(2013, 5, 10)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 3, 1), new DateTime(2013, 4, 1)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList[0].StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList[0].EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_IncludesEntireRange_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 6, 10, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1, 13, 13, 13), new DateTime(2013, 5, 10, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 3, 1, 13, 13, 13), new DateTime(2013, 4, 1, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);
            Assert.AreEqual(newRangesList[0].StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList[0].EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheLeft_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 20), new DateTime(2013, 1, 30)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 10), new DateTime(2013, 1, 19)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 9)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.LastOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.FirstOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheLeft_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 20, 13, 13, 13), new DateTime(2013, 1, 30, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 10, 13, 13, 13), new DateTime(2013, 1, 19, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 9, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.LastOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.FirstOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheLeft_SingleDateRanges_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 4), new DateTime(2013, 1, 4)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 3), new DateTime(2013, 1, 3)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 2), new DateTime(2013, 1, 2)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 1)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.LastOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.FirstOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheLeft_SingleDateRanges_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 4, 13, 13, 13), new DateTime(2013, 1, 4, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 3, 13, 13, 13), new DateTime(2013, 1, 3, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 2, 13, 13, 13), new DateTime(2013, 1, 2, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 1, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.LastOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.FirstOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheRight_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 9)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 10), new DateTime(2013, 1, 19)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 20), new DateTime(2013, 1, 31)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1), new DateTime(2013, 2, 10)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheRight_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 9, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 10, 13, 13, 13), new DateTime(2013, 1, 19, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 20, 13, 13, 13), new DateTime(2013, 1, 31, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 2, 1, 13, 13, 13), new DateTime(2013, 2, 10, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheRight_SingleDateRanges_AssertRangesMerged()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();

            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1), new DateTime(2013, 1, 1)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 2), new DateTime(2013, 1, 2)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 3), new DateTime(2013, 1, 3)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 4), new DateTime(2013, 1, 4)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }

        [TestMethod]
        public void Test_DateRanges_MergesAllRangesToTheRight_SingleDateRanges_AssertRangesMerged_WithTimeComponent()
        {
            List<CalendarDateRange> newRangesList = new List<CalendarDateRange>();

            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 1, 13, 13, 13), new DateTime(2013, 1, 1, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 2, 13, 13, 13), new DateTime(2013, 1, 2, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 3, 13, 13, 13), new DateTime(2013, 1, 3, 15, 15, 15)));
            newRangesList.Add(new CalendarDateRange(new DateTime(2013, 1, 4, 13, 13, 13), new DateTime(2013, 1, 4, 15, 15, 15)));

            foreach (CalendarDateRange dateRange in newRangesList)
            {
                this.dateRangeCollection.AddDateRange(dateRange);
            }

            Assert.AreEqual<int>(1, this.dateRangeCollection.Count);

            Assert.AreEqual(newRangesList.FirstOrDefault().StartDate, this.dateRangeCollection[0].StartDate);
            Assert.AreEqual(newRangesList.LastOrDefault().EndDate, this.dateRangeCollection[0].EndDate);
        }
    }
}