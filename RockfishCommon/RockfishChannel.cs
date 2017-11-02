﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using Rhino;
using Rhino.Geometry;

namespace RockfishCommon
{
  public class RockfishChannel : IDisposable
  {
    private ChannelFactory<IRockfishService> m_factory;
    private IRockfishService m_channel;
    //private NetNamedPipeBinding m_binding;
    private BasicHttpBinding m_binding;
    private EndpointAddress m_endpoint;
    private readonly object m_locker;
    private bool m_disposed;

    /// <summary>
    /// Public constructor
    /// </summary>
    public RockfishChannel()
    {
      m_locker = new object();
      m_disposed = false;
    }

    /// <summary>
    /// Public creator
    /// </summary>
    public bool Create(string hostName)
    {
      if (string.IsNullOrEmpty(hostName))
        return false;

      var rc = false;
      try
      {
        //m_binding = new NetNamedPipeBinding();
        //var uri = "net.pipe://localhost/mcneel/rockfishserver/5/server/pipe";
        m_binding = new BasicHttpBinding();
        var uri = $"http://{hostName}:8000/mcneel/rockfish/5/server/basic";
        m_endpoint = new EndpointAddress(uri);
        m_factory = new ChannelFactory<IRockfishService>(m_binding, m_endpoint);
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
          var result = m_channel.Echo(str);
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
      if (IsValid)
      {
        try
        {
          var result = m_channel.IntersectBreps(inBrep0, inBrep1, tolerance);
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
      if (IsValid)
      {
        try
        {
          var result = m_channel.PolylineFromPoints(inPoints, minimumDistance);
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
      if (IsValid)
      {
        try
        {
          var result = m_channel.CreateMeshFromBrep(inBrep, bSmooth);
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
      if (ex is FaultException)
      {
        ThrowFaultException((FaultException)ex);
      }
      else if (ex is CommunicationException)
      {
        ThrowCommunicationException((CommunicationException)ex);
      }
      else if (ex is TimeoutException)
      {
        ThrowTimeoutException((TimeoutException)ex);
      }
      else
      {
        ThrowGeneralException(ex);
      }
    }

    /// <summary>
    /// Handles creation exceptions
    /// </summary>
    private static void ThrowCreationException(Exception ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "There was a problem creating the communication channel.";
      throw new RockfishChannelException(message);
    }

    /// <summary>
    /// Handles fault exceptions
    /// </summary>
    private static void ThrowFaultException(FaultException ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      throw new RockfishChannelException(ex.Message);
    }

    /// <summary>
    /// Handles communication exceptions
    /// </summary>
    private static void ThrowCommunicationException(CommunicationException ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "There was a problem communicating with the service.";
      throw new RockfishChannelException(message);
    }

    /// <summary>
    /// Handles timeout exceptions
    /// </summary>
    private static void ThrowTimeoutException(TimeoutException ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "The service operation has timed out.";
      throw new RockfishChannelException(message);
    }

    /// <summary>
    /// Handles generic exceptions
    /// </summary>
    private static void ThrowGeneralException(Exception ex)
    {
      Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
      const string message = "An unknown exception has occurred.";
      throw new RockfishChannelException(message);
    }

    /// <inheritdoc />
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
            if (null != m_channel)
            {
              ((IClientChannel) m_channel).Abort();
              m_channel = null;
            }

            if (null != m_factory)
            {
              m_factory.Abort();
              m_factory = null;
            }

            m_endpoint = null;
            m_binding = null;
          }

          m_disposed = true;
        }
      }
    }
  }
}
