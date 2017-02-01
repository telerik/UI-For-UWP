using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a collection of <see cref="Brush"/> objects that target a particular <see cref="PaletteVisualPart"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    [ContentProperty(Name = "Brushes")]
    public class PaletteEntryCollection
    {
        private List<Brush> brushes;
        private string seriesFamily;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteEntryCollection"/> class.
        /// </summary>
        public PaletteEntryCollection()
        {
            this.brushes = new List<Brush>();
        }

        /// <summary>
        /// Gets the collection that contains all the entries for the associated visual part.
        /// </summary>
        public List<Brush> Brushes
        {
            get
            {
                return this.brushes;
            }
        }

        /// <summary>
        /// Gets or sets the family of series this collection is applicable to.
        /// </summary>
        public string SeriesFamily
        {
            get
            {
                return this.seriesFamily;
            }
            set
            {
                this.seriesFamily = value;
            }
        }
    }
}
