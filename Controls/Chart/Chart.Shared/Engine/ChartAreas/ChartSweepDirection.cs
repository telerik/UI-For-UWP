using System;
using System.Linq;

namespace Telerik.Charting
{
    /// <summary>Specifies the direction in which an elliptical arc is drawn.</summary>
    public enum ChartSweepDirection : int 
    {
        /// <summary>Arcs are drawn in a counterclockwise (negative-angle) direction.</summary>
        Counterclockwise = 0,

        /// <summary>Arcs are drawn in a clockwise (positive-angle) direction.</summary>
        Clockwise = 1,
    }
}