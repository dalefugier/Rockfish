using System;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using RockfishCommon;

namespace RockfishClient
{
  public class RockfishEchoCommand : Command
  {
    public override string EnglishName => "RockfishEcho";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var message = "Hello Rhino!";
      var rc = RhinoGet.GetString("String to echo", false, ref message);
      if (rc != Result.Success)
        return rc;

      try
      {
        var channel = new RockfishChannel();
        channel.Create();
        message = channel.Echo(message);
        channel.Dispose();
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
