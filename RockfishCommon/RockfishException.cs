using System;
using System.Runtime.Serialization;

namespace RockfishCommon
{
  /// <summary>
  /// RockfishChannelException exception class
  /// </summary>
  public class RockfishException : ApplicationException
  {
    public RockfishException()
    {
    }

    public RockfishException(string message)
      : base(message)
    {
    }

    public RockfishException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected RockfishException(SerializationInfo info, StreamingContext context)
    {
    }
  }
}
