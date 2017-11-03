using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using RockfishCommon;

namespace RockfishClient.Commands
{
  /// <summary>
  /// RockfishEcho command
  /// </summary>
  [System.Runtime.InteropServices.Guid("F474A13B-DA09-427F-933A-2DC57D1E12D0")]
  public class RockfishEchoCommand : Command
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public override string EnglishName => "RockfishEcho";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var rc = RockfishClientPlugIn.Instance.VerifyServerHostName();
      if (rc != Result.Success)
        return rc;

      var message = "Hello Rhino!";
      rc = RhinoGet.GetString("String to echo", false, ref message);
      if (rc != Result.Success)
        return rc;

      try
      {
        var host_name = RockfishClientPlugIn.Instance.ServerHostName();
        using (var channel = new RockfishChannel())
        {
          channel.Create(host_name);
          message = channel.Echo(message);
        }
      }
      catch (Exception ex)
      {
        RhinoApp.WriteLine(ex.Message);
        return Result.Failure;
      }

      RhinoApp.WriteLine(message);

      return Result.Success;
    }
  }
}
