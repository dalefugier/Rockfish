using System.ServiceModel;

namespace RockfishCommon
{
  [ServiceContract]
  public interface IRockfishService
  {
    /// <summary>
    /// Simple test to see if the RockFish service is operational
    /// </summary>
    /// <param name="str"></param>
    /// <returns>The echoed string if successful.</returns>
    [OperationContract]
    string Echo(string str);

    /// <summary>
    /// Intersects two Brep objects and returns the intersection curves
    /// </summary>
    /// <param name="inBrep0">The first Brep.</param>
    /// <param name="inBrep1">The second Brep.</param>
    /// <param name="tolerance">The intersection tolerance.</param>
    /// <returns>The intersection curves if successful.</returns>
    [OperationContract]
    RockfishGeometry[] IntersectBreps(RockfishGeometry inBrep0, RockfishGeometry inBrep1, double tolerance);

    /// <summary>
    /// Creates a polyline curve from an array of points.
    /// Also removes points from the array if their common 
    /// distance exceeds a specified threshold.
    /// </summary>
    /// <param name="inPoints">The array of points.</param>
    /// <param name="minimumDistance">
    /// Minimum allowed distance among a pair of points. 
    /// If points are closer than this, only one of them will be kept.
    /// </param>
    /// <returns>The polyline curve if successful.</returns>
    [OperationContract]
    RockfishGeometry PolylineFromPoints(RockfishPoint[] inPoints, double minimumDistance);

    /// <summary>
    /// Constructs a mesh from a Brep.
    /// </summary>
    /// <param name="inBrep">The Brep.</param>
    /// <param name="bSmooth">
    /// If true, a smooth mesh is returned. 
    /// If false, then a coarse mesh is returned.
    /// </param>
    /// <returns>The mesh if successful.</returns>
    [OperationContract]
    RockfishGeometry CreateMeshFromBrep(RockfishGeometry inBrep, bool bSmooth);
  }
}
