/* Microsoft Permissive License (Ms-PL)
 * Derived from <a href="http://deepearth.codeplex.com" />
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 * 
 * 1. Definitions
 *  The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
 *  A "contribution" is the original software, or any additions or changes to the software.
 *  A "contributor" is any person that distributes its contribution under this license.
 *  "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 *  
 * 2. Grant of Rights
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 * 
 * 3. Conditions and Limitations
 * (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 * (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
 * */

using System;
using Telerik.Geospatial;
using Telerik.UI.Drawing;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// The OGC Spatial Reference requirements.
    /// </summary>
    public class SpatialReference : ISpatialReference
    {
        /// <summary>
        /// Half of PI.
        /// </summary>
        public const double HalfPI = 0.159154943091895;

        /// <summary>
        /// Degrees of one radiant.
        /// </summary>
        public const double RadiansToDegrees = 57.2957795130823;

        /// <summary>
        /// Gets or sets the measurement units used to define the angles of a spheroid or ellipse associated with a specific datum.
        /// The datum is WGS 1984 and the unit of measurement is a degree.
        /// </summary>
        public double AngularUnitOfMeasurement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the authority body that defines the standards for the spatial reference parameters.
        /// The Spatial Reference is WGS 1984 and the authority is EPSG:4326.
        /// </summary>
        public string Authority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line of longitude at the center of a map projection generally used as the basis for constructing the projection.
        /// </summary>
        public double CentralMeridian
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the horizontal datum,
        /// which corresponds to the procedure used to measure positions on the surface of the Earth.
        /// </summary>
        public string Datum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the horizontal datum,
        /// which corresponds to the procedure used to measure positions on the surface of the Earth.
        /// </summary>
        public string DatumAuthority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value added to all "x" values in the rectangular coordinate for a map projection. 
        /// This value frequently is assigned to eliminate negative numbers.
        /// Expressed in the unit of measure identified in Planar Coordinate Units.
        /// </summary>
        public double FalseEasting
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value added to all "y" values in the rectangular coordinates for a map projection. 
        /// This value frequently is assigned to eliminate negative numbers. 
        /// Expressed in the unit of measure identified in Planar Coordinate Units.
        /// </summary>
        public double FalseNorthing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets coordinate system based on latitude and longitude.
        /// Some geographic coordinate systems are Latitude/Longitude, and some are Longitude/Latitude.
        /// You can find out which this is by examining the axes. 
        /// You should also check the angular units, since not all geographic coordinate systems use degrees.
        /// </summary>
        public string GeoGcs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the latitude chosen as the origin of rectangular coordinate for a map projection.
        /// </summary>
        public double LatitudeOfOrigin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets logical X offset to centre of earth.
        /// </summary>
        public double OffsetX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets logical Y offset to centre of earth.
        /// </summary>
        public double OffsetY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the meridian used to take longitude measurements from. 
        /// The units of the longitude must be inferred from the context.
        /// </summary>
        public double Primem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the meridian used to take longitude measurements from. 
        /// The units of the longitude must be inferred from the context.
        /// </summary>
        public string PrimemAuthority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a projection from geographic coordinates to projected coordinates.
        /// </summary>
        public string ProjectionAuthority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the real world coordinate scale at a given longitude.
        /// </summary>
        public double ScaleX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the real world coordinate scale at a given latitude.
        /// </summary>
        public double ScaleY
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a spheroid, which is an approximation of the Earth's surface as a squashed sphere.
        /// </summary>
        public double SpheroidRadius
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a spheroid, which is an approximation of the Earth's surface as a squashed sphere.
        /// </summary>
        public double SpheroidFlattening
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a spheroid, which is an approximation of the Earth's surface as a squashed sphere.
        /// </summary>
        public string SpheroidAuthority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the line of constant latitude at which the surface of the Earth
        /// and the plane or developable surface intersect.
        /// </summary>
        public double StandardParallel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the authority body that defines the unit of measurement i.e. European Petroleum Survey Group (EPSG).
        /// The unit of measurement is usually degrees and the authority for the datum the map uses, WGS 1984 is EPSG:4326.
        /// </summary>
        public string UnitAuthority
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum Latitude this tile source supports.
        /// </summary>
        public double MinLatitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum Latitude this tile source supports.
        /// </summary>
        public double MaxLatitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum Longitude this tile source supports.
        /// </summary>
        public double MinLongitude
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum Longitude this tile source supports.
        /// </summary>
        public double MaxLongitude
        {
            get;
            set;
        }

        /// <summary>
        /// Converts a geographical coordinate (Longitude, Latitude) to a logical Point (0->1).
        /// </summary>
        /// <param name="geographicPoint">The geographical coordinate (Longitude, Latitude).</param>
        /// <returns>The logical Point.</returns>
        public virtual DoublePoint ConvertGeographicToLogicalCoordinate(Location geographicPoint)
        {
            double d = Math.Sin(geographicPoint.Latitude * this.AngularUnitOfMeasurement);

            DoublePoint point = new DoublePoint()
                {
                    X = (geographicPoint.Longitude * this.AngularUnitOfMeasurement * this.ScaleX) + this.OffsetX,
                    Y = (0.5 * Math.Log((1.0 + d) / (1.0 - d)) * this.ScaleY) + this.OffsetY
                };

            if (point.Y > 1)
            {
                point.Y = 1;
            }

            return point;
        }

        /// <summary>
        /// Converts a logical Point (0->1) to a geographical coordinate (Longitude, Latitude).
        /// </summary>
        /// <param name="logicalPoint">The logical Point.</param>
        /// <returns>The geographical coordinate (Longitude, Latitude).</returns>
        public virtual Location ConvertLogicalToGeographicCoordinate(DoublePoint logicalPoint)
        {
            return new Location(
                Math.Atan(Math.Sinh((logicalPoint.Y - this.OffsetY) / this.ScaleY)) * RadiansToDegrees,
                (logicalPoint.X - this.OffsetX) * RadiansToDegrees / this.ScaleX);
        }
    }
}
