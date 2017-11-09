using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input.Custom;
using RockfishCommon;

namespace RockfishClient.Commands
{
  /// <summary>
  /// RF_CreateMeshFromBrep command
  /// </summary>
  public class CreateMeshFromBrepCommand : Command
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public override string EnglishName => "RF_CreateMeshFromBrep";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var rc = RockfishClientPlugIn.VerifyServerHostName();
      if (rc != Result.Success)
        return rc;

      var go = new GetObject();
      go.SetCommandPrompt("Select surfaces or polysurfaces mesh");
      go.GeometryFilter = ObjectType.Surface | ObjectType.PolysrfFilter;
      go.SubObjectSelect = false;
      go.GetMultiple(1, 0);
      if (go.CommandResult() != Result.Success)
        return go.CommandResult();

      var breps = new List<Brep>();
      foreach (var obj_ref in go.Objects())
      {
        if (ObjectType.Brep == obj_ref.Geometry().ObjectType)
        {
          var brep = obj_ref.Brep();
          if (null != brep)
            breps.Add(brep);
        }
        else if (ObjectType.Extrusion == obj_ref.Geometry().ObjectType)
        {
          var extrusion = obj_ref.Geometry() as Extrusion;
          var brep = extrusion?.ToBrep(true);
          if (null != brep)
            breps.Add(brep);
        }
      }

      if (0 == breps.Count)
        return Result.Cancel;

      try
      {
        using (var channel = new RockfishClientChannel())
        {
          channel.Create();
          foreach (var brep in breps)
          {
            var in_brep = new RockfishGeometry(brep);
            var out_mesh = channel.CreateMeshFromBrep(in_brep, false);
            if (null != out_mesh?.Mesh)
            {
              var object_id = doc.Objects.AddMesh(out_mesh.Mesh);
              var rhino_object = doc.Objects.Find(object_id);
              rhino_object?.Select(true);
            }
          }
        }
      }
      catch (Exception ex)
      {
        RhinoApp.WriteLine(ex.Message);
        return Result.Failure;
      }

      doc.Views.Redraw();

      return Result.Success;
    }
  }
}