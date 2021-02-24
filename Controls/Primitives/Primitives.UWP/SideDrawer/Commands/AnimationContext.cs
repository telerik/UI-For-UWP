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

        internal DrawerLocation DrawerLocation { get; set; }

        internal DrawerTransition DrawerTransition { get; set; }

        internal bool IsGenerated
        {
            get
            {
                return this.MainContentStoryBoard != null && this.MainContentStoryBoardReverse != null && this.DrawerStoryBoard != null && this.DrawerStoryBoardReverse != null;
            }        
        }

        internal bool IsValid(DrawerLocation location, DrawerTransition transition)
        {
            return this.IsGenerated && this.DrawerLocation == location && this.DrawerTransition == transition;
        }
    }
}
