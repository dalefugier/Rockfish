using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using RockfishCommon;

namespace RockfishServer
{
  /// <summary>
  /// IRockfishService definition
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
    /// <param name="inBrep0">The first Brep.</param>
    /// <param name="inBrep1">The second Brep.</param>
    /// <param name="tolerance">The intersection tolerance.</param>
    /// <returns>The intersection curves if successful.</returns>
    public RockfishGeometry[] IntersectBreps(RockfishGeometry inBrep0, RockfishGeometry inBrep1, double tolerance)
    {
      RhinoApp.WriteLine("IntersectBreps request received");

      if (null == inBrep0?.Brep || null == inBrep1?.Brep)
        return new RockfishGeometry[0];

      var rc = Intersection.BrepBrep(inBrep0.Brep, inBrep1.Brep, tolerance, out Curve[] curves, out Point3d[] points);
      if (!rc)
        return new RockfishGeometry[0];

      var out_curves = new RockfishGeometry[curves.Length];
      for (var i = 0; i < curves.Length; i++)
        out_curves[i] = new RockfishGeometry(curves[i]);

      return out_curves;
    }

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
    public RockfishGeometry PolylineFromPoints(RockfishPoint[] inPoints, double minimumDistance)
    {
      RhinoApp.WriteLine("PolylineFromPoints request received");

      if (null == inPoints || 0 == inPoints.Length)
        return null;

      var points = new List<Point3d>(inPoints.Length);
      points.AddRange(from point in inPoints where null != point select point.ToPoint3d());

      var culled_points = Point3d.SortAndCullPointList(points, minimumDistance);
      if (null == culled_points || culled_points.Length < 2)
        return null;

      var polyline_curve = new PolylineCurve(culled_points);
      var out_curve = new RockfishGeometry(polyline_curve);

      return out_curve;
    }

    /// <summary>
    /// Constructs a mesh from a Brep.
    /// </summary>
    /// <param name="inBrep">The Brep.</param>
    /// <param name="bSmooth">
    /// If true, a smooth mesh is returned. 
    /// If false, then a coarse mesh is returned.
    /// </param>
    /// <returns>The mesh if successful.</returns>
    public RockfishGeometry CreateMeshFromBrep(RockfishGeometry inBrep, bool bSmooth)
    {
      RhinoApp.WriteLine("CreateMeshFromBrep request received");

      if (null == inBrep?.Brep)
        return null;

      var mp = bSmooth ? MeshingParameters.Smooth : MeshingParameters.Coarse;
      var meshes = Mesh.CreateFromBrep(inBrep.Brep, mp);
      if (null == meshes || 0 == meshes.Length)
        return null;

      var brep_mesh = new Mesh();
      foreach (var mesh in meshes)
        brep_mesh.Append(mesh);

      var out_mesh = new RockfishGeometry(brep_mesh);

      return out_mesh;
    }
  }
}
