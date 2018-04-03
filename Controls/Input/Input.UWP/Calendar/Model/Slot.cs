using System;
using System.Globalization;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// A class that represents destination slot.
    /// </summary>
    public class Slot : ViewModelBase
    {
        internal RadRect layoutSlot;

        private DateTime end;
        private DateTime start;

        /// <summary>
        /// Initializes a new instance of the <see cref="Slot"/> class.
        /// </summary>
        public Slot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Slot"/> class with the given start and end time.
        /// </summary>
        /// <param name="start">The start time of the instance.</param>
        /// <param name="end">The end of the instance.</param>
        public Slot(DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Slot"/> class with the given start time and duration.
        /// </summary>
        /// <param name="start">The start time of the instance.</param>
        /// <param name="duration">The duration of the instance.</param>
        public Slot(DateTime start, TimeSpan duration)
        {
            this.start = start;
            this.end = start.Add(duration);
        }

        /// <summary>
        /// Gets or sets the start <see cref="DateTime"/> of the <see cref="Slot"/>.
        /// </summary>
        public DateTime Start
        {
            get
            {
                return this.start;
            }
            set
            {
                if (this.start != value)
                {
                    this.start = value;
                    this.OnPropertyChanged(nameof(this.Start));
                }
            }
        }

        /// <summary>
        /// Gets or sets the end <see cref="DateTime"/> of the <see cref="Slot"/>.
        /// </summary>
        public DateTime End
        {
            get
            {
                return this.end;
            }
            set
            {
                if (this.end != value)
                {
                    this.end = value;
                    this.OnPropertyChanged(nameof(this.End));
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Start: {0}, End: {1}", this.start, this.end);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var slot = obj as Slot;
            if (slot != null)
            {
                return this.Start == slot.Start && this.End == slot.End;
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
