using System;
using Rhino;
using Rhino.Geometry;
using RockfishCommon;

namespace RockfishServer
{
  public class RockfishService : IRockfishService
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public RockfishService()
    {
      // TODO...
    }

    /// <inheritdoc />
    public string Echo(string str)
    {
      RhinoApp.WriteLine("Echo received : " + str);
      return "Echo from Server : " + str;
    }

    public Guid AddCurve(RockfishGeometry curve)
    {
      if (curve?.Curve != null)
      {
        var doc = RhinoDoc.ActiveDoc;
        var guid = doc.Objects.AddCurve(curve.Curve);
        doc.Views.Redraw();
        return guid;
      }
      return Guid.Empty;
    }

    public RockfishGeometry[] IntersectBreps(RockfishGeometry brep0, RockfishGeometry brep1, double tolerance)
    {
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
