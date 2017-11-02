using Rhino;
using Rhino.Geometry;
using RockfishCommon;

namespace RockfishServer
{
  /// <summary>
  /// IRockfishService implementation
  /// </summary>
  public class RockfishService : IRockfishService
  {
    /// <summary>
    /// Simple test to see if the RockFish service is operational
    /// </summary>
    /// <param name="str"></param>
    /// <returns>The echoed string if successful.</returns>
    public string Echo(string str)
    {
      RhinoApp.WriteLine("Echo request received : " + str);
      var host_name = RockfishServerPlugIn.Instance.ServerHostName();
      var rc = $"Echo from \"{host_name}\" : {str}";
      return rc;
    }

    /// <summary>
    /// Intersects two Brep objects and returns the intersection curves
    /// </summary>
    /// <param name="brep0">The first Brep.</param>
    /// <param name="brep1">The second Brep.</param>
    /// <param name="tolerance">The intersection tolerance.</param>
    /// <returns>The intersection curves if successful.</returns>
    public RockfishGeometry[] IntersectBreps(RockfishGeometry brep0, RockfishGeometry brep1, double tolerance)
    {
      RhinoApp.WriteLine("IntersectBreps request received");

      if (null == brep0?.Brep || null == brep1?.Brep)
        return new RockfishGeometry[0];

      var rc = Rhino.Geometry.Intersect.Intersection.BrepBrep(brep0.Brep, brep1.Brep, tolerance, out Curve[] out_curves, out Point3d[] out_points);
      if (!rc)
        return new RockfishGeometry[0];

      var result = new RockfishGeometry[out_curves.Length];
      for (var i = 0; i < out_curves.Length; i++)
        result[i] = new RockfishGeometry(out_curves[i]);

      return result;
    }
  }
}
