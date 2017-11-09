using System;
using RockfishCommon;

namespace RockfishConsole
{
  /// <summary>
  /// ConsoleChannel class
  /// </summary>
  internal class ConsoleChannel : RockfishChannel
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    public ConsoleChannel(string hostName)
    {
      HostName = hostName;
    }

    /// <summary>
    /// RockfishChannel.ClientId override
    /// </summary>
    public override string ClientId => Environment.MachineName;

    /// <summary>
    /// RockfishChannel.HostName override
    /// </summary>
    public override string HostName { get; }
  }
}
