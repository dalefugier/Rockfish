using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input.Custom;
using RockfishCommon;

namespace RockfishClient.Commands
{
  /// <summary>
  /// RockfishPolylineFromPoints command
  /// </summary>
  [System.Runtime.InteropServices.Guid("50076313-0259-464A-9BC7-5B4FBC26A764")]
  public class RockfishPolylineFromPointsCommand : Command
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public override string EnglishName => "RockfishPolylineFromPoints";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var rc = RockfishClientPlugIn.Instance.VerifyServerHostName();
      if (rc != Result.Success)
        return rc;

      var go = new GetObject();
      go.SetCommandPrompt("Select points");
      go.GeometryFilter = ObjectType.Point;
      go.SubObjectSelect = false;
      go.GetMultiple(2, 0);
      if (go.CommandResult() != Result.Success)
        return go.CommandResult();

      var in_points = new List<RockfishPoint>(go.ObjectCount);
      foreach (var obj_ref in go.Objects())
      {
        var point = obj_ref.Point();
        if (null != point)
          in_points.Add(new RockfishPoint(point.Location));
      }

      if (in_points.Count < 2)
        return Result.Cancel;

      RockfishGeometry out_curve;
      try
      {
        var host_name = RockfishClientPlugIn.Instance.ServerHostName();
        using (var channel = new RockfishChannel())
        {
          channel.Create(host_name);
          out_curve = channel.PolylineFromPoints(in_points.ToArray(), doc.ModelAbsoluteTolerance);
        }
      }
      catch (Exception ex)
      {
        RhinoApp.WriteLine(ex.Message);
        return Result.Failure;
      }

      if (null != out_curve?.Curve)
      { 
        var object_id = doc.Objects.AddCurve(out_curve.Curve);
        doc.Views.Redraw();
      }

      return Result.Success;
    }
  }
}