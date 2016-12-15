using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    public interface INumericalRange
    {
        double Min { get; }

        double Max { get; }

        double Step { get; }
    }
}
