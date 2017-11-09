using System;
using RockfishCommon;

namespace RockfishServer
{
  /// <summary>
  /// RockfishRecord class
  /// </summary>
  public class RockfishRecord : IDisposable
  {
    private bool m_disposed;

    /// <summary>
    /// Public constructor
    /// </summary>
    public RockfishRecord(RockfishHeader header)
    {
      Header = header;
    }

    /// <summary>
    /// Gets the referenced header.
    /// </summary>
    public RockfishHeader Header { get; }

    /// <summary>
    /// IDisposable.Dispose override
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the class.
    /// </summary>
    private void Dispose(bool disposing)
    {
      if (!m_disposed)
      {
        if (disposing)
        {
          RockfishLog.TheLog.Enqueue(Header);
          m_disposed = true;
        }
      }
    }
  }
}
