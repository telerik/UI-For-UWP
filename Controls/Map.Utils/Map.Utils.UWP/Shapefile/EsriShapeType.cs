using System.Diagnostics.CodeAnalysis;
namespace Telerik.Geospatial
{
    internal enum EsriShapeType
    {
        /// <summary>
        /// Null Shape.
        /// </summary>
        NullShape = 0,

        /// <summary>
        /// Point.
        /// </summary>
        Point = 1,

        /// <summary>
        /// Polyline.
        /// </summary>
        Polyline = 3,

        /// <summary>
        /// Polygon.
        /// </summary>
        Polygon = 5,

        /// <summary>
        /// Multipoint.
        /// </summary>
        Multipoint = 8,

        /// <summary>
        /// PointZ.
        /// </summary>
        PointZ = 11,

        /// <summary>
        /// PolylineZ.
        /// </summary>
        PolylineZ = 13,

        /// <summary>
        /// PolygonZ.
        /// </summary>
        PolygonZ = 15,

        /// <summary>
        /// MultipointZ.
        /// </summary>
        MultipointZ = 18,

        /// <summary>
        /// PointM.
        /// </summary>
        PointM = 21,

        /// <summary>
        /// PolylineM.
        /// </summary>
        PolylineM = 23,

        /// <summary>
        /// PolygonM.
        /// </summary>
        PolygonM = 25,

        /// <summary>
        /// MultipointM.
        /// </summary>
        MultipointM = 28,

        /// <summary>
        /// Multipatch.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Consistent with casing in ESRI specification.")]
        Multipatch = 31
    }
}
