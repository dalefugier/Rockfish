using RockfishCommon;

namespace RockfishClient
{
  /// <summary>
  /// RockfishClientChannel class
  /// </summary>
  internal class RockfishClientChannel : RockfishChannel
  {
    /// <summary>
    /// RockfishChannel.ClientId override
    /// </summary>
    public override string ClientId => RockfishClientPlugIn.ClientId;

    /// <summary>
    /// RockfishChannel.HostName override
    /// </summary>
    public override string HostName => RockfishClientPlugIn.ServerHostName();
  }
}
