using System;
using System.Diagnostics;
using System.ServiceModel;

namespace RockfishCommon
{
  public abstract class RockfishChannel : IDisposable
  {
    private const int MAX_BUFFER = 4194304; // 4 MB

    private readonly object m_locker;
    private ChannelFactory<IRockfishService> m_factory;
    private IRockfishService m_channel;
    private bool m_disposed;

    /// <summary>
    /// Protected constructor
    /// </summary>
    protected RockfishChannel()
    {
      m_locker = new object();
      m_disposed = false;
    }

    /// <summary>
    /// Gets an id that allows events to be aggregated by user. 
    /// </summary>
    public abstract string ClientId { get; }

    /// <summary>
    /// Gets the host name or ip address of the target server.
    /// </summary>
    public abstract string HostName { get; }

    /// <summary>
    /// Public creation method
    /// </summary>
    public bool Create()
    {
      if (string.IsNullOrEmpty(HostName))
        return false;

      var rc = false;
      try
      {
        var binding = new BasicHttpBinding
        {
          MaxBufferSize = MAX_BUFFER,
          MaxReceivedMessageSize = MAX_BUFFER
        };

        var uri = $"http://{HostName}:8000/mcneel/rockfish/5/server/basic";
        var endpoint = new EndpointAddress(uri);
        m_factory = new ChannelFactory<IRockfishService>(binding, endpoint);
        m_channel = m_factory.CreateChannel();

        rc = true;
      }
      catch (Exception ex)
      {
        ThrowCreationException(ex);
        Dispose();
      }
      return rc;
    }

    /// <summary>
    /// Simple test to see if the RockFish service is operational
    /// </summary>
    /// <param name="str"></param>
    /// <returns>The echoed string if successful.</returns>
    public string Echo(string str)
    {
      if (IsValid)
      {
        try
        {
          var header = new RockfishHeader(ClientId);
          var result = m_channel.Echo(header, str);
          return result;
        }
        catch (Exception ex)
        {
          HandleException(ex);
          Dispose();
        }
      }
      return null;
    }

    /// <summary>
    /// Intersects two Brep objects and returns the intersection curves
    /// </summary>
    /// <param name="inBrep0">The first Brep.</param>
    /// <param name="inBrep1">The second Brep.</param>
    /// <param name="tolerance">The intersection tolerance.</param>
    /// <returns>The intersection curves if successful.</returns>
    public RockfishGeometry[] IntersectBreps(RockfishGeometry inBrep0, RockfishGeometry inBrep1, double tolerance)
    {
      if (null == inBrep0?.Brep || null == inBrep1?.Brep)
        return null;

      if (IsValid)
      {
        try
        {
          var header = new RockfishHeader(ClientId);
          var result = m_channel.IntersectBreps(header, inBrep0, inBrep1, tolerance);
          return result;
        }
        catch (Exception ex)
        {
          HandleException(ex);
          Dispose();
        }
      }
      return new RockfishGeometry[0];
    }

    /// <summary>
    /// Creates a polyline curve from an array of points.
    /// Also removes points from the array if their common
    /// distance exceeds a specified threshold.
    /// </summary>
    /// <param name="inPoints">The array of points.</param>
    /// <param name="minimumDistance">
    /// Minimum allowed distance among a pair of points. 
    /// If points are closer than this, only one of them will be kept.
    /// </param>
    /// <returns>The polyline curve if successful.</returns>
    public RockfishGeometry PolylineFromPoints(RockfishPoint[] inPoints, double minimumDistance)
    {
      if (null == inPoints || 0 == inPoints.Length)
        return null;

      if (IsValid)
      {
        try
        {
          var header = new RockfishHeader(ClientId);
          var result = m_channel.PolylineFromPoints(header, inPoints, minimumDistance);
          return result;
        }
        catch (Exception ex)
        {
          HandleException(ex);
          Dispose();
        }
      }
      return null;
    }

    /// <summary>
    /// Constructs a mesh from a Brep.
    /// </summary>
    /// <param name="inBrep">The Brep.</param>
    /// <param name="bSmooth">
    /// If true, a smooth mesh is returned. 
    /// If false, then a coarse mesh is returned.
    /// </param>
    /// <returns>The mesh if successful.</returns>
    public RockfishGeometry CreateMeshFromBrep(RockfishGeometry inBrep, bool bSmooth)
    {
      if (null == inBrep?.Brep)
        return null;

      if (IsValid)
      {
        try
        {
          var header = new RockfishHeader(ClientId);
          var result = m_channel.CreateMeshFromBrep(header, inBrep, bSmooth);
          return result;
        }
        catch (Exception ex)
        {
          HandleException(ex);
          Dispose();
        }
      }
      return null;
    }

    /// <summary>
    /// Object validator
    /// </summary>
    public bool IsValid => null != m_factory && null != m_channel && false == m_disposed;

    /// <summary>
    /// Exception handler
    /// </summary>
    private static void HandleException(Exception ex)
    {
      Console.WriteLine(ex.Message);
      var exception = ex as FaultException;
      if (exception != null)
        ThrowFaultException(exception);
      else if (ex is CommunicationException)
        ThrowCommunicationException((CommunicationException)ex);
      else if (ex is TimeoutException)
        ThrowTimeoutException((TimeoutException)ex);
      else
        ThrowGeneralException(ex);
    }

    /// <summary>
    /// Handles creation exceptions
    /// </summary>
    private static void ThrowCreationException(Exception ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "There was a problem creating the communication channel.";
      throw new RockfishException(message);
    }

    /// <summary>
    /// Handles fault exceptions
    /// </summary>
    private static void ThrowFaultException(FaultException ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      throw new RockfishException(ex.Message);
    }

    /// <summary>
    /// Handles communication exceptions
    /// </summary>
    private static void ThrowCommunicationException(CommunicationException ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "There was a problem communicating with the service.";
      throw new RockfishException(message);
    }

    /// <summary>
    /// Handles timeout exceptions
    /// </summary>
    private static void ThrowTimeoutException(TimeoutException ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "The service operation has timed out.";
      throw new RockfishException(message);
    }

    /// <summary>
    /// Handles generic exceptions
    /// </summary>
    private static void ThrowGeneralException(Exception ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "An unknown exception has occurred.";
      throw new RockfishException(message);
    }

    /// <summary>
    /// IDisposable interface
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    private void Dispose(bool disposing)
    {
      if (!m_disposed)
      {
        if (disposing)
        {
          lock (m_locker)
          {
            // Close the communication channel. Note, despite the `suspicious` compiler
            // warning, you can do an explicit cast. The ChannelFactory.CreateChannel() 
            // signature returns the IRockfishService interface. But is also inherits
            // from the IChannel interface under the hood.
            var channel = (IClientChannel) m_channel;
            try
            {
              channel.Close();
            }
            catch
            {
              channel.Abort();
            }
            m_channel = null;

            // Close the channel factory
            if (m_factory.State == CommunicationState.Closed)
            {
              try
              {
                m_factory.Close();
              }
              catch
              {
                m_factory.Abort();
              }
              m_factory = null;
            }
          }

          m_disposed = true;
        }
      }
    }
  }
}
