using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands
{
    /// <summary>
    /// Defines a context which holds the animations used to open/close the drawer.
    /// </summary>
    public class AnimationContext
    {
        /// <summary>
        /// Gets or sets the storyboard which holds the animations for the main content used while opening the drawer.
        /// </summary>
        public Storyboard MainContentStoryBoard { get; set; }

        /// <summary>
        /// Gets or sets the storyboard which holds the animations for the main content used while closing the drawer.
        /// </summary>
        public Storyboard MainContentStoryBoardReverse { get; set; }

        /// <summary>
        /// Gets or sets the storyboard which holds the animations for the drawer used while opening the drawer.
        /// </summary>
        public Storyboard DrawerStoryBoard { get; set; }

        /// <summary>
        /// Gets or sets the storyboard which holds the animations for the drawer used while closing the drawer.
        /// </summary>
        public Storyboard DrawerStoryBoardReverse { get; set; }

        internal bool IsGenerated
        {
            get
            {
                return this.MainContentStoryBoard != null && this.MainContentStoryBoardReverse != null && this.DrawerStoryBoard != null && this.DrawerStoryBoardReverse != null;
            }        
        }
    }
}
