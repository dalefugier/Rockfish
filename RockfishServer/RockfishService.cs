using System;
using Rhino;
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
    }

    /// <inheritdoc />
    public string Echo(string str)
    {
      RhinoApp.WriteLine("Echo received : " + str);
      return "Echo from Server : " + str;
    }

    public Guid AddCurve(RockfishCurve rockfishCurve)
    {
      if (rockfishCurve?.Curve != null)
      {
        var doc = RhinoDoc.ActiveDoc;
        var guid = doc.Objects.AddCurve(rockfishCurve.Curve);
        doc.Views.Redraw();
        return guid;
      }
      return Guid.Empty;
    }
  }
}
