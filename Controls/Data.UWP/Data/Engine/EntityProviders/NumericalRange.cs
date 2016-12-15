using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    public class NumericalRange : INumericalRange
    {
        public NumericalRange(double min, double max, double step)
        {
            this.Min = min;
            this.Max = max;
            this.Step = step;
        }

        public double Max
        {
            get; private set;
        }

        public double Min
        {
            get; private set;
        }

        public double Step
        {
            get; private set;
        }
    }
}
