using System;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input;
using RockfishCommon;

namespace RockfishClient
{
  public class RockfishAddCurveCommand : Command
  {
    public override string EnglishName => "RockfishAddCurve";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      const ObjectType filter = ObjectType.Curve;
      var rc = RhinoGet.GetOneObject("Select curve", false, filter, out var objref);
      if (rc != Result.Success || objref == null)
        return rc;

      var curve = objref.Curve();
      if (curve == null)
        return Result.Failure;

      var rf_curve = new RockfishGeometry(curve);

      var guid = Guid.Empty;
      try
      {
        var channel = new RockfishChannel();
        channel.Create();
        guid = channel.AddCurve(rf_curve);
        channel.Dispose();
      }
      catch (Exception ex)
      {
        RhinoApp.WriteLine(ex.Message);
        return Result.Failure;
      }

      RhinoApp.WriteLine(guid.ToString());

      return Result.Success;
    }
  }
}