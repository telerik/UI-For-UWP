using System;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Primitives
{
    /// <summary>
    /// Represents the custom Control implementation used to visualize the current item within a <see cref="RadListView"/> component.
    /// </summary>
    public class ListViewCurrencyControl : RadControl, IAnimated
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewCurrencyControl" /> class.
        /// </summary>
        public ListViewCurrencyControl()
        {
            this.DefaultStyleKey = typeof(ListViewCurrencyControl);

            this.IsTabStop = false;
            this.IsHitTestVisible = false;
        }

        object IAnimated.Container
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool IAnimated.IsAnimating
        {
            get;
            set;
        }
    }
}
