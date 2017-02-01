using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines the different visual parts within a chart visual tree.
    /// </summary>
    public enum PaletteVisualPart
    {
        /// <summary>
        /// Fill-related part.
        /// </summary>
        Fill,

        /// <summary>
        /// Stroke-related part.
        /// </summary>
        Stroke,

        /// <summary>
        /// Special (or additional) fill part. Applicable to some special chart series like CandleStick.
        /// </summary>
        SpecialFill,

        /// <summary>
        /// Special (or additional) stroke part. Applicable to some special chart series like CandleStick.
        /// </summary>
        SpecialStroke
    }
}
