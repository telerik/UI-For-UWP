using System.Globalization;
namespace Telerik.Geospatial
{
    /// <summary>
    /// Represents a geographical location.
    /// </summary>
    public struct Location
    {
        /// <summary>
        /// An empty location.
        /// </summary>
        public static readonly Location Empty = new Location(0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> struct.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public Location(double latitude, double longitude)
            : this()
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Gets or sets the latitude value of the location.
        /// </summary>
        public double Latitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the longitude value of the location.
        /// </summary>
        public double Longitude
        {
            get;
            set;
        }

        /// <summary>
        /// Checks whether two <see cref="Location"/> values are equal.
        /// </summary>
        public static bool operator ==(Location location1, Location location2)
        {
            return location1.Equals(location2);
        }

        /// <summary>
        /// Checks whether two <see cref="Location"/> values are not equal.
        /// </summary>
        public static bool operator !=(Location location1, Location location2)
        {
            return !location1.Equals(location2);
        }

        /// <summary>
        /// Checks whether two <see cref="Location"/> values are equal.
        /// </summary>
        public static bool Equals(Location location1, Location location2)
        {
            return location1.Latitude == location2.Latitude && location1.Longitude == location2.Longitude;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentUICulture, "{0}, {1}", this.Latitude, this.Longitude);
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
            if (obj == null || !(obj is Location))
            {
                return false;
            }

            return Equals(this, (Location)obj);
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
