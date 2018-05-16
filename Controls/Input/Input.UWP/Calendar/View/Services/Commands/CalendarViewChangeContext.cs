using System;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a context, passed to the commands associated with changing calendar view (move to previous/next view, move to upper/lower view, move to date).
    /// </summary>
    public class CalendarViewChangeContext
    {
        internal int navigationStep;

        /// <summary>
        /// Gets or sets the animation storyboard associated with the navigation action.
        /// </summary>
        public Storyboard AnimationStoryboard
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the date associated with the navigation action.
        /// </summary>
        /// <remarks>
        /// This is applicable only for <see cref="MoveToDateCommand"/> and <see cref="MoveToLowerViewCommand"/> commands at the moment.
        /// </remarks>
        public DateTime? Date
        {
            get;
            internal set;
        }
    }
}
