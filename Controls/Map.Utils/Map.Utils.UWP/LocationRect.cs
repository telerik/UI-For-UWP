namespace Telerik.Geospatial
{
    /// <summary>
    /// Represents a structure that defines a Geographical rectangle, defined by two <see cref="Location"/> values.
    /// </summary>
    public struct LocationRect
    {
        /// <summary>
        /// An empty rect.
        /// </summary>
        public static readonly LocationRect Empty = new LocationRect(Location.Empty, Location.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationRect"/> struct.
        /// </summary>
        /// <param name="northwest">The northwest.</param>
        /// <param name="southeast">The southeast.</param>
        public LocationRect(Location northwest, Location southeast)
            : this()
        {
            this.Northwest = northwest;
            this.Southeast = southeast;
        }

        /// <summary>
        /// Gets the northwest value of the rect.
        /// </summary>
        public Location Northwest
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the southeast value of the rect.
        /// </summary>
        public Location Southeast
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the north value of the rect.
        /// </summary>
        public double North
        {
            get
            {
                return this.Northwest.Latitude;
            }
        }

        /// <summary>
        /// Gets the east value of the rect.
        /// </summary>
        public double East
        {
            get
            {
                return this.Southeast.Longitude;
            }
        }

        /// <summary>
        /// Gets the south value of the rect.
        /// </summary>
        public double South
        {
            get
            {
                return this.Southeast.Latitude;
            }
        }

        /// <summary>
        /// Gets the west value of the rect.
        /// </summary>
        public double West
        {
            get
            {
                return this.Northwest.Longitude;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current rect is empty.
        /// </summary>
        public bool IsEmpty 
        {
            get
            {
                return this.North == this.South && this.East == this.West;
            }
        }

        /// <summary>
        /// Checks whether two <see cref="LocationRect"/> values are equal.
        /// </summary>
        public static bool operator ==(LocationRect locationRect1, LocationRect locationRect2)
        {
            return locationRect1.Equals(locationRect2);
        }

        /// <summary>
        /// Checks whether two <see cref="LocationRect"/> values are not equal.
        /// </summary>
        public static bool operator !=(LocationRect locationRect1, LocationRect locationRect2)
        {
            return !locationRect1.Equals(locationRect2);
        }

        /// <summary>
        /// Checks whether two <see cref="LocationRect"/> values are equal.
        /// </summary>
        public static bool Equals(LocationRect locationRect1, LocationRect locationRect2)
        {
            return locationRect1.Northwest == locationRect2.Northwest && locationRect1.Southeast == locationRect2.Southeast;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is LocationRect))
            {
                return false;
            }

            return Equals(this, (LocationRect)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
