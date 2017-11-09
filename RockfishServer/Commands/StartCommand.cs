using Rhino;
using Rhino.Commands;

namespace RockfishServer.Commands
{
  /// <summary>
  /// RockfishStart command
  /// </summary>
  public class StartCommand : Command
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public override string EnglishName => "RockfishStart";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      if (!RockfishServerPlugIn.CheckForAdministrator())
        return Result.Cancel;

      var service = RockfishServiceHost.TheServiceHost;
      if (service.IsRunning)
      {
        RhinoApp.WriteLine("Rockfish service already running.");
        return Result.Success;
      }

      // Start the service
      var rc = service.Start();

      if (rc)
      {
        // Start the activity log (if not disabled)
        RockfishLog.TheLog.Start();
      }

      return rc ? Result.Success : Result.Failure;
    }
  }
}