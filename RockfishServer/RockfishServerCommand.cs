using System;
using System.Security.Principal;
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
    /// Gets the command name.
    /// </summary>
    public override string EnglishName => "RockfishServer";

    /// <summary>
    /// Called by Rhino when the user wants to run the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      if (!IsAdministrator())
      {
        RhinoApp.WriteLine("To start the service, please run Rhino as Administrator.");  
        return Result.Cancel;
      }

      // Toggle service operation
      if (null == m_service_host)
        Start();
      else
        Stop();

      return Result.Success;
    }

    /// <summary>
    /// Detects whether or not Rhino is running "as Administrtor".
    /// Adminstrative privledges are required to HTTP binding.
    /// </summary>
    private static bool IsAdministrator()
    {
      try
      {
        var user = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(user);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
      }
      catch
      {
        // ignored
      }
      return false;
    }

    /// <summary>
    /// Starts the server service.
    /// </summary>
    private void Start()
    {
      try
      {
        //  m_service_host = new ServiceHost(
        //    typeof(RockfishService),
        //    new Uri("net.pipe://localhost/mcneel/rockfishserver/5/server"));

        m_service_host = new ServiceHost(
          typeof(RockfishService), 
          new Uri("http://localhost:8000/mcneel/rockfish/5/server"));

         RhinoApp.WriteLine("Service host created.");
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create service host.");
        Stop();
        return;
      }

      try
      {
        //m_service_host.AddServiceEndpoint(
        //  typeof(IRockfishService), 
        //  new NetNamedPipeBinding(), "pipe");

        m_service_host.AddServiceEndpoint(
          typeof(IRockfishService),
          new BasicHttpBinding(), "basic");

        RhinoApp.WriteLine("Service end point created.");
      }
      catch
      {
        RhinoApp.WriteLine("Failed to create service end point.");
        Stop();
        return;
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
        return;
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
        return;
      }

      RhinoApp.WriteLine("Service started.");
    }

    /// <summary>
    /// Stop the service.
    /// </summary>
    private void Stop()
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
