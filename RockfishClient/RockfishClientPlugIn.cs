using System.Net;
using Rhino;
using Rhino.Commands;

namespace RockfishClient
{
  /// <summary>
  /// RockfishClientPlugIn plug-in class
  /// </summary>
  public class RockfishClientPlugIn : Rhino.PlugIns.PlugIn
  {
    private string m_server_host_name;

    public RockfishClientPlugIn()
    {
      Instance = this;
    }

    /// <summary> 
    /// Gets the only instance of the RockfishClientPlugIn plug-in.
    /// </summary>
    public static RockfishClientPlugIn Instance
    {
      get; private set;
    }

    /// <summary>
    /// Called by commands to verify the server host name is set.
    /// </summary>
    public Result VerifyServerHostName()
    {
      var rc = string.IsNullOrEmpty(ServerHostName());
      if (rc)
        RhinoApp.WriteLine("Run the \"RockfishSetServer\" command to set the server host name.");
      return rc ? Result.Cancel : Result.Success;
    }

    /// <summary>
    /// Gets the server host name.
    /// </summary>
    public string ServerHostName()
    {
      if (!string.IsNullOrEmpty(m_server_host_name))
        return m_server_host_name;

      if (Settings.TryGetString("ServerHostName", out string host_name))
        m_server_host_name = LookupHostName(host_name);

      return m_server_host_name;
    }

    /// <summary>
    /// Sets the server host name.
    /// </summary>
    /// <param name="serverHostName">The server host name.</param>
    public void SetServerHostName(string serverHostName)
    {
      m_server_host_name = serverHostName;
      Settings.SetString("ServerHostName", m_server_host_name);
    }

    /// <summary>
    /// Resolves a host name to a canonical host name.
    /// </summary>
    public static string LookupHostName(string hostName)
    {
      if (string.IsNullOrEmpty(hostName))
        return null;

      // If the string parses as an IP address, return.
      if (IPAddress.TryParse(hostName, out IPAddress address))
        return hostName;

      try
      {
        var host_entry = Dns.GetHostEntry(hostName);
        return host_entry.HostName;
      }
      catch
      {
        // ignored
      }

      return null;
    }
  }
}