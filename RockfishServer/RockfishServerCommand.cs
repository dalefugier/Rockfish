using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Rhino;
using Rhino.Commands;
using RockfishCommon;

namespace RockfishServer
{
  public class RockfishServerCommand : Command
  {
    private ServiceHost m_service_host;

    /// <summary>
    /// Constructor
    /// </summary>
    public RockfishServerCommand()
    {
    }

    /// <inheritdoc />
    public override string EnglishName => "RockfishServer";

    /// <inheritdoc />
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      if (null == m_service_host)
        Start();
      else
        Stop();
      return Result.Success;
    }

    /// <summary>
    /// Starts the server service.
    /// </summary>
    bool Start()
    {
      try
      {
        m_service_host = new ServiceHost(
          typeof(RockfishService),
          new Uri("net.pipe://localhost/mcneel/rockfishserver/1/server"));
        RhinoApp.WriteLine("Service host created.");
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create service host.");
        Stop();
        return false;
      }

      try
      {
        m_service_host.AddServiceEndpoint(
          typeof(IRockfishService), 
          new NetNamedPipeBinding(), "pipe");
        RhinoApp.WriteLine("Service end point created.");
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create service end point.");
        Stop();
        return false;
      }

      try
      {
        var debug_behavior = m_service_host.Description.Behaviors.Find<ServiceDebugBehavior>();
        if (null == debug_behavior)
        {
          debug_behavior = new ServiceDebugBehavior {IncludeExceptionDetailInFaults = true};
          m_service_host.Description.Behaviors.Add(debug_behavior);
        }
        else
        {
          debug_behavior.IncludeExceptionDetailInFaults = true;
        }
        RhinoApp.WriteLine("Service debug behavior created.");
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create Rockfish service debug behavior.");
        Stop();
        return false;
      }

      try
      {
        m_service_host.Open();
        RhinoApp.WriteLine("Service opened.");
      }
      catch
      {
        RhinoApp.WriteLine("Failed to open Rockfish service.");
        Stop();
        return false;
      }

      RhinoApp.WriteLine("Service started.");
      return true;
    }

    /// <summary>
    /// Stop the service.
    /// </summary>
    void Stop()
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
      RhinoApp.WriteLine("Service stopped.");
    }
  }
}
