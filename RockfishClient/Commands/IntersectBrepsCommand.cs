using System;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input.Custom;
using RockfishCommon;

namespace RockfishClient.Commands
{
  /// <summary>
  /// RF_IntersectBreps command
  /// </summary>
  [System.Runtime.InteropServices.Guid("9570A22E-B5A8-485E-82FF-33346993A01A")]
  public class IntersectBrepsCommand : Command
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public override string EnglishName => "RF_IntersectBreps";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var rc = RockfishClientPlugIn.Instance.VerifyServerHostName();
      if (rc != Result.Success)
        return rc;

      var go = new GetObject();
      go.SetCommandPrompt("Select two surfaces or polysurfaces to intersect");
      go.GeometryFilter = ObjectType.Surface | ObjectType.PolysrfFilter;
      go.SubObjectSelect = false;
      go.GetMultiple(2, 2);
      if (go.CommandResult() != Result.Success)
        return go.CommandResult();

      var brep0 = go.Object(0).Brep();
      var brep1 = go.Object(1).Brep();
      if (null == brep0 || null == brep1)
        return Result.Failure;

      var in_brep0 = new RockfishGeometry(brep0);
      var in_brep1 = new RockfishGeometry(brep1);

      RockfishGeometry[] out_curves;
      try
      {
        var host_name = RockfishClientPlugIn.Instance.ServerHostName();
        using (var channel = new RockfishChannel())
        {
          channel.Create(host_name);
          out_curves = channel.IntersectBreps(in_brep0, in_brep1, doc.ModelAbsoluteTolerance);
        }
      }
      catch (Exception ex)
      {
        RhinoApp.WriteLine(ex.Message);
        return Result.Failure;
      }

      foreach (var out_curve in out_curves)
      {
        if (null != out_curve?.Curve)
        {
          var object_id = doc.Objects.AddCurve(out_curve.Curve);
          var rhino_object = doc.Objects.Find(object_id);
          rhino_object?.Select(true);
        }
      }

      doc.Views.Redraw();

      return Result.Success;
    }
  }
}