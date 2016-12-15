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

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Mercator is the projection used by most online maps including Virtual Earth, GMaps and Yahoo
    /// It maintains Vertical resolution while expanding the earth horizontally to fill
    /// The effect is that items at the poles appear much larger then those of equal size at the equator.
    /// </summary>
    public class MercatorProjection : SpatialReference
    {
        /// <summary>
        /// Initializes a new instance of the MercatorProjection class.
        /// Mercator is the projection used by most online maps including Virtual Earth, GMaps and Yahoo.
        /// </summary>
        public MercatorProjection()
        {
            this.GeoGcs = "GCS_WGS_1984";
            this.Datum = "WGS_1984";
            this.SpheroidRadius = 6378137.00D;
            this.SpheroidFlattening = 298.257223563D;
            this.Primem = 0.0D;
            this.AngularUnitOfMeasurement = 0.017453292519943295D;
            this.FalseEasting = 0.0D;
            this.FalseNorthing = 0.0D;
            this.CentralMeridian = 0.0D;
            this.LatitudeOfOrigin = 0.0D;
            this.UnitAuthority = "Meter";

            this.ScaleX = SpatialReference.HalfPI;
            this.ScaleY = -SpatialReference.HalfPI;
            this.OffsetX = 0.5;
            this.OffsetY = 0.5;

            this.MinLatitude = -85.05112878D;
            this.MaxLatitude = 85.05112878D;
            this.MinLongitude = -180D;
            this.MaxLongitude = 180D;
        }
    }
}
