using System;
using System.Runtime.Serialization;

namespace RockfishCommon
{
  public class RockfishChannelException : ApplicationException
  {
    public RockfishChannelException()
    {
    }

    public RockfishChannelException(string message)
      : base(message)
    {
    }

    public RockfishChannelException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected RockfishChannelException(SerializationInfo info, StreamingContext context)
    {
    }
  }
}
