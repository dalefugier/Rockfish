using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Rhino;
using RockfishCommon;

namespace RockfishServer
{
  /// <summary>
  /// RockfishServiceHost class (singleton)
  /// </summary>
  internal class RockfishServiceHost
  {
    private const int MAX_BUFFER = 4194304; // 4 MB
    private ServiceHost m_service_host;

    /// <summary>
    /// Private constructor
    /// </summary>
    private RockfishServiceHost()
    {
      // ignored
    }

    /// <summary>
    /// The one and only RockfishServiceHost object.
    /// </summary>
    static RockfishServiceHost g_the_service_host;

    /// <summary>
    /// Gets the one and only instance of the RockfishServiceHost object.
    /// </summary>
    public static RockfishServiceHost TheServiceHost => g_the_service_host ?? (g_the_service_host = new RockfishServiceHost());

    /// <summary>
    /// Returns true if the service is running.
    /// </summary>
    public bool IsRunning => null != m_service_host;

    /// <summary>
    /// Starts the service.
    /// </summary>
    public bool Start()
    {
      // Try creating the service host
      try
      {
        //m_service_host = new ServiceHost(
        //  typeof(RockfishService),
        //  new Uri("net.pipe://localhost/mcneel/rockfishserver/5/server"));

        m_service_host = new ServiceHost(
          typeof(RockfishService),
          new Uri("http://localhost:8000/mcneel/rockfish/5/server"));
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create Rockfish service host.");
        Stop();
        return false;
      }

      // Try creating the binding and add a servic endpoint
      try
      {
        //var binding = new NetNamedPipeBinding
        //{
        //  MaxBufferSize = MAX_BUFFER,
        //  MaxReceivedMessageSize = MAX_BUFFER
        //};
        //
        //m_service_host.AddServiceEndpoint(typeof(IRockfishService), binding, "pipe");

        var binding = new BasicHttpBinding
        {
          MaxBufferSize = MAX_BUFFER,
          MaxReceivedMessageSize = MAX_BUFFER
        };

        m_service_host.AddServiceEndpoint(typeof(IRockfishService), binding, "basic");
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create Rockfish service end point.");
        Stop();
        return false;
      }

      // Try creating the service debug behavior
      try
      {
        var debug_behavior = m_service_host.Description.Behaviors.Find<ServiceDebugBehavior>();
        if (null == debug_behavior)
        {
          debug_behavior = new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true };
          m_service_host.Description.Behaviors.Add(debug_behavior);
        }
        else
        {
          debug_behavior.IncludeExceptionDetailInFaults = true;
        }
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create Rockfish service debug behavior.");
        Stop();
        return false;
      }

      // Try opening the service
      try
      {
        m_service_host.Open();
      }
      catch
      {
        RhinoApp.WriteLine("Failed to open Rockfish service.");
        Stop();
        return false;
      }

      RhinoApp.WriteLine("Rockfish service started.");
      return true;
    }

    /// <summary>
    /// Stops the service.
    /// </summary>
    public void Stop()
    {
      if (null != m_service_host)
      {
        try
        {
          m_service_host.Close();
        }
        catch
        {
          // ignored
        }
        m_service_host = null;
      }

      RhinoApp.WriteLine("Rockfish service stopped.");
    }
  }
}
