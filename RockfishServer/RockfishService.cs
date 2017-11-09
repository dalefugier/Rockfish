using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using RockfishCommon;

namespace RockfishServer
{
  /// <summary>
  /// IRockfishService implementation
  /// </summary>
  [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class RockfishService : IRockfishService
  {
    /// <summary>
    /// Simple test to see if the RockFish service is operational.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="str"></param>
    /// <returns>The echoed string if successful.</returns>
    public string Echo(RockfishHeader header, string str)
    {
      if (null == header)
        throw new FaultException("RockfishHeader is null");

      header.Method = nameof(IntersectBreps);
      RhinoApp.WriteLine("{0} request received from {1}.", header.Method, header.ClientId);

      using (var item = new RockfishRecord(header))
      {
        var host_name = RockfishServerPlugIn.ServerHostName();
        var rc = $"Echo from \"{host_name}\" : {str}";

        header.Succeeded = true;
        return rc;
      }
    }

    /// <summary>
    /// Intersects two Brep objects and returns the intersection curves.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="inBrep0">The first Brep.</param>
    /// <param name="inBrep1">The second Brep.</param>
    /// <param name="tolerance">The intersection tolerance.</param>
    /// <returns>The intersection curves if successful.</returns>
    public RockfishGeometry[] IntersectBreps(RockfishHeader header, RockfishGeometry inBrep0, RockfishGeometry inBrep1, double tolerance)
    {
      if (null == header)
        throw new FaultException("RockfishHeader is null");

      header.Method = nameof(IntersectBreps);
      RhinoApp.WriteLine("{0} request received from {1}.", header.Method, header.ClientId);

      using (var item = new RockfishRecord(header))
      {
        if (null == inBrep0?.Brep || null == inBrep1?.Brep)
          throw new FaultException("Brep is null");

        var rc = Intersection.BrepBrep(inBrep0.Brep, inBrep1.Brep, tolerance, out Curve[] curves, out Point3d[] points);
        if (!rc || null == curves || 0 == curves.Length)
          throw new FaultException("Unable to intersect two Breps.");

        var out_curves = new RockfishGeometry[curves.Length];
        for (var i = 0; i < curves.Length; i++)
          out_curves[i] = new RockfishGeometry(curves[i]);

        header.Succeeded = true;
        return out_curves;
      }
    }

    /// <summary>
    /// Creates a polyline curve from an array of points.
    /// Also removes points from the array if their common
    /// distance exceeds a specified threshold.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="inPoints">The array of points.</param>
    /// <param name="minimumDistance">
    /// Minimum allowed distance among a pair of points. 
    /// If points are closer than this, only one of them will be kept.
    /// </param>
    /// <returns>The polyline curve if successful.</returns>
    public RockfishGeometry PolylineFromPoints(RockfishHeader header, RockfishPoint[] inPoints, double minimumDistance)
    {
      if (null == header)
        throw new FaultException("RockfishHeader is null");

      header.Method = nameof(IntersectBreps);
      RhinoApp.WriteLine("{0} request received from {1}.", header.Method, header.ClientId);

      using (var item = new RockfishRecord(header))
      {
        if (null == inPoints || 0 == inPoints.Length)
          throw new FaultException("Points array is null or empty.");

        var points = new List<Point3d>(inPoints.Length);
        points.AddRange(from point in inPoints where null != point select point.ToPoint3d());

        var culled_points = Point3d.SortAndCullPointList(points, minimumDistance);
        if (null == culled_points || culled_points.Length < 2)
          throw new FaultException("Points array is null or empty.");

        var polyline_curve = new PolylineCurve(culled_points);
        var out_curve = new RockfishGeometry(polyline_curve);

        header.Succeeded = true;
        return out_curve;
      }
    }

    /// <summary>
    /// Constructs a mesh from a Brep.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="inBrep">The Brep.</param>
    /// <param name="bSmooth">
    /// If true, a smooth mesh is returned. 
    /// If false, then a coarse mesh is returned.
    /// </param>
    /// <returns>The mesh if successful.</returns>
    public RockfishGeometry CreateMeshFromBrep(RockfishHeader header, RockfishGeometry inBrep, bool bSmooth)
    {
      if (null == header)
        throw new FaultException("RockfishHeader is null");

      header.Method = nameof(IntersectBreps);
      RhinoApp.WriteLine("{0} request received from {1}.", header.Method, header.ClientId);

      using (var item = new RockfishRecord(header))
      {
        if (null == inBrep?.Brep)
          throw new FaultException("Brep is null");

        var mp = bSmooth ? MeshingParameters.Smooth : MeshingParameters.Coarse;
        var meshes = Mesh.CreateFromBrep(inBrep.Brep, mp);
        if (null == meshes || 0 == meshes.Length)
          throw new FaultException("Unable to construct mesh from Brep.");

        var brep_mesh = new Mesh();
        foreach (var mesh in meshes)
          brep_mesh.Append(mesh);

        var out_mesh = new RockfishGeometry(brep_mesh);

        header.Succeeded = true;
        return out_mesh;
      }
    }
  }
}
