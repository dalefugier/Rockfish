using Rhino;
using Rhino.Commands;

namespace RockfishServer.Commands
{
  /// <summary>
  /// StopCommand command
  /// </summary>
  public class StopCommand : Command
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public override string EnglishName { get; } = "RockfishStop";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      if (!RockfishServerPlugIn.CheckForAdministrator())
        return Result.Cancel;

      // Stop the activity log
      RockfishLog.TheLog.Stop();

      // Stop the service
      RockfishServiceHost.TheServiceHost.Stop();

      return Result.Success;
    }
  }
}