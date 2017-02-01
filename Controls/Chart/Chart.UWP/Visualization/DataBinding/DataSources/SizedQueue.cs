using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class SizedQueue : IEnumerable<double>
    {
        public double RunningSum;
        public int Size;

        private List<double> items = new List<double>(16);

        public SizedQueue(int size)
        {
            this.Size = size;
        }

        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public double Last
        {
            get
            {
                return this.items[this.items.Count - 1];
            }
        }

        public double this[int index]
        {
            get
            {
                return this.items[index];
            }
        }

        public void EnqueueItem(double item)
        {
            if (this.Size <= this.items.Count && this.Size > 0)
            {
                this.DequeueItem();
            }

            this.Enqueue(item);
            this.RunningSum += item;
        }

        public double Peek()
        {
            return this.items[0];
        }

        public double DequeueItem()
        {
            double result = this.Dequeue();
            this.RunningSum -= result;

            return result;
        }

        public IEnumerator<double> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private void Enqueue(double value)
        {
            // add value at the end
            this.items.Add(value);
        }

        private double Dequeue()
        {
            // get the value at the beginning
            double value = this.items[0];
            this.items.RemoveAt(0);

            return value;
        }
    }
}