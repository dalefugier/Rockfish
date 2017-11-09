using System;
using System.Linq;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace RockfishServer.Commands
{
  /// <summary>
  /// RockfishConfig command
  /// </summary>
  public class ConfigCommand : Command
  {
    /// <summary>
    /// Gets the command name.
    /// </summary>
    public override string EnglishName => "RockfishConfig";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var log_type = RockfishServerPlugIn.LogType();

      var go = new GetOption();
      go.SetCommandPrompt("Configuration options");
      go.AddOptionEnumList("Logging", log_type);
      while (true)
      {
        var res = go.Get();
        if (res == GetResult.Option)
        {
          var option = go.Option();
          if (null != option)
          {
            var list = Enum.GetValues(typeof(RockfishLog.LogType)).Cast<RockfishLog.LogType>().ToList();
            log_type = list[option.CurrentListOptionIndex];
          }
          continue;
        }
        break;
      }

      RockfishServerPlugIn.SetLogType(log_type);

      return Result.Success;
    }
  }
}