using System;
using System.Globalization;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// A class that represents destination slot.
    /// </summary>
    public class Slot : ViewModelBase, ICopyable<Slot>
    {
        internal RadRect layoutSlot;

        private DateTime end;
        private DateTime start;
        private bool isReadOnly;

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
        /// Gets or sets a value indicating whether this slot is read only.
        /// </summary>
        /// <value>
        /// True if this slot is read only; otherwise, False.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }
            set
            {
                if (this.isReadOnly != value)
                {
                    this.isReadOnly = value;
                    this.OnPropertyChanged(nameof(this.IsReadOnly));
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
        public virtual Slot Copy()
        {
            var slot = new Slot();
            slot.CopyFrom(this);
            return slot;
        }

        /// <summary>
        /// <b>Deep</b> copies all properties from <paramref name="other"/> to this <see cref="Slot"/>.
        /// </summary>
        /// <param name="other">The <see cref="Slot"/> which properties are copied.</param>
        public virtual void CopyFrom(Slot other)
        {
            var otherSlot = other as Slot;
            if (otherSlot == null)
            {
                return;
            }

            this.Start = other.Start;
            this.End = other.End;
            this.IsReadOnly = other.IsReadOnly;
            this.layoutSlot = other.layoutSlot;
        }

        internal bool IntersectsWith(Slot slot)
        {
            if (slot == null)
            {
                return false;
            }

            return (slot.Start <= this.start && this.start < slot.End) || (this.start <= slot.Start && slot.Start < this.end);
        }
    }
}
