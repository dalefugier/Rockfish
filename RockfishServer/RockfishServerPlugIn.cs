using System;
using System.Net;
using System.Net.Sockets;

namespace RockfishServer
{
  /// <summary>
  /// RockfishServerPlugIn plug-in class
  /// </summary>
  public class RockfishServerPlugIn : Rhino.PlugIns.PlugIn
  {
    private string m_server_host_name;

    /// <summary>
    /// Public constructor (called by Rhino)
    /// </summary>
    public RockfishServerPlugIn()
    {
      Instance = this;
    }

    /// <summary>
    /// Gets the one and only instance of the RockfishServerPlugIn object.
    /// </summary>
    public static RockfishServerPlugIn Instance
    {
      get; private set;
    }

    /// <summary>
    /// Returns the local host name
    /// </summary>
    public string ServerHostName()
    {
      if (!string.IsNullOrEmpty(m_server_host_name))
        return m_server_host_name;

      m_server_host_name = GetHostName();
      if (!string.IsNullOrEmpty(m_server_host_name))
        return m_server_host_name;

      m_server_host_name = GetIpAddress();
      if (!string.IsNullOrEmpty(m_server_host_name))
        return m_server_host_name;

      m_server_host_name = Environment.MachineName;
      return m_server_host_name;
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