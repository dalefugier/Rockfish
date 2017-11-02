using Rhino;
using Rhino.Commands;
using Rhino.Input.Custom;
using RockfishCommon;

namespace RockfishClient.Commands
{
  public class RockfishSetServer : Command
  {
    public override string EnglishName => "RockfishSetServer";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var server_host_name = RockfishClientPlugIn.Instance.ServerHostName();

      for (var i = 0; i < 3; i++)
      {
        var gs = new GetString();
        gs.SetCommandPrompt("Server host name or IP address");
        gs.SetDefaultString(server_host_name);
        gs.Get();
        if (gs.CommandResult() != Result.Success)
          return gs.CommandResult();

        var name = gs.StringResult().Trim();
        if (string.IsNullOrEmpty(name))
        {
          RhinoApp.WriteLine("Server host name or IP address cannot be empty.");
          continue;
        }

        var host_name = RockfishClientPlugIn.LookupHostName(name);
        if (string.IsNullOrEmpty(host_name))
        {
          RhinoApp.WriteLine("Unable to resolve host name \"{0}\".", host_name);
          continue;
        }

        var found = false;
        try
        {
          using (var channel = new RockfishChannel())
          {
            channel.Create(host_name);
            var echo = channel.Echo("Echo");
            found = !string.IsNullOrEmpty(echo);
          }
        }
        catch
        {
          // ignored
        }

        if (!found)
        {
          RhinoApp.WriteLine("Unable to connect to server \"{0}\".", host_name);
          continue;
        }

        RockfishClientPlugIn.Instance.SetServerHostName(host_name);
        break;
      }

      return Result.Success;
    }
  }
}