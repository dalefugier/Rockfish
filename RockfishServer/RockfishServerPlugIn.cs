using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using Rhino;
using Rhino.PlugIns;

namespace RockfishServer
{
  /// <summary>
  /// RockfishServerPlugIn plug-in class
  /// </summary>
  public class RockfishServerPlugIn : PlugIn
  {
    private static string g_server_host_name;
    private static RockfishLog.LogType g_log_type;

    /// <summary>
    /// Public constructor (called by Rhino).
    /// </summary>
    public RockfishServerPlugIn()
    {
      g_log_type = RockfishLog.LogType.Disabled;
      ThePlugIn = this;
    }

    /// <summary>
    /// Gets the one and only instance of the RockfishServerPlugIn object.
    /// </summary>
    public static RockfishServerPlugIn ThePlugIn
    {
      get; private set;
    }

    /// <summary>
    /// Called by Rhino when the plug-in is being loaded.
    /// </summary>
    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
      g_log_type = Settings.GetEnumValue<RockfishLog.LogType>("LogType", RockfishLog.LogType.Disabled);
      return LoadReturnCode.Success;
    }

    /// <summary>
    /// Gets the activity logging type.
    /// </summary>
    public static RockfishLog.LogType LogType()
    {
      return g_log_type;
    }

    /// <summary>
    /// Sets the activity logging type.
    /// </summary>
    public static void SetLogType(RockfishLog.LogType logType)
    {
      g_log_type = logType;
      ThePlugIn.Settings.SetEnumValue<RockfishLog.LogType>("LogType", g_log_type);

      if (RockfishServiceHost.TheServiceHost.IsRunning)
      {
        if (g_log_type == RockfishLog.LogType.Disabled)
          RockfishLog.TheLog.Stop();
        else
          RockfishLog.TheLog.Start();
      }
    }

    /// <summary>
    /// Detects whether or not Rhino is running "as Administrator".
    /// Adminstrative privledges are required for HTTP binding.
    /// </summary>
    public static bool CheckForAdministrator()
    {
      var rc = false;
      try
      {
        var user = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(user);
        rc = principal.IsInRole(WindowsBuiltInRole.Administrator);
      }
      catch
      {
        // ignored
      }

      if (!rc)
        RhinoApp.WriteLine("RockfishServer requires that Rhino is run with Administrator privledges.");

      return rc;
    }

    /// <summary>
    /// Returns the local host name.
    /// </summary>
    public static string ServerHostName()
    {
      if (!string.IsNullOrEmpty(g_server_host_name))
        return g_server_host_name;

      g_server_host_name = GetHostName();
      if (!string.IsNullOrEmpty(g_server_host_name))
        return g_server_host_name;

      g_server_host_name = GetIpAddress();
      if (!string.IsNullOrEmpty(g_server_host_name))
        return g_server_host_name;

      g_server_host_name = Environment.MachineName;
      return g_server_host_name;
    }

    /// <summary>
    /// Gets the local host name.
    /// </summary>
    private static string GetHostName()
    {
      try
      {
        var host_name = Dns.GetHostName();
        return host_name;
      }
      catch
      {
        // ignored
      }
      return null;
    }

    /// <summary>
    /// Gets the local IP address.
    /// </summary>
    private static string GetIpAddress()
    {
      try
      {
        var host_entry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var address in host_entry.AddressList)
        {
          if (address.AddressFamily == AddressFamily.InterNetwork)
            return address.ToString();
        }
      }
      catch
      {
        // ignored
      }
      return null;
    }

  }
}