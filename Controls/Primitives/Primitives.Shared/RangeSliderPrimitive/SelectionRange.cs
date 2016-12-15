using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.RangeSlider
{
    internal struct SelectionRange
    {
        private double start;
        private double end;

        public SelectionRange(double start, double end)
        {
            this.start = start;
            this.end = end;
        }

        public double Start
        {
            get
            {
                return this.start;
            }

            set
            {
                this.start = value;
            }
        }
        public double End
        {
            get
            {
                return this.end;
            }

            set
            {
                this.end = value;
            }
        }
    }
}
