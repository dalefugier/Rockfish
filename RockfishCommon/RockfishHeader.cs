using System;
using System.Runtime.Serialization;
using System.Text;

namespace RockfishCommon
{
  /// <summary>
  /// RockfishHeader class
  /// This class contains some meta data that might be useful
  /// to the server. 
  /// </summary>
  [DataContract]
  public class RockfishHeader
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="clientId">The client id.</param>
    public RockfishHeader(string clientId)
    {
      ClientId = clientId;
      Date = DateTime.UtcNow; // UTC
      Method = string.Empty;
      Succeeded = false;
    }

    /// <summary>
    /// Gets or sets an id that allows events to be aggregated by user. 
    /// </summary>
    [DataMember]
    public string ClientId { get; set; }

    /// <summary>
    /// The current date and time universal time (UTC).
    /// </summary>
    [DataMember]
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the mathod named called on the server. 
    /// </summary>
    [DataMember]
    public string Method  { get; set; }

    /// <summary>
    /// Indicates whether or not the server mathod completed successfully.
    /// </summary>
    [DataMember]
    public bool Succeeded { get; set; }

    /// <summary>
    /// Returns a string to be used as a heading for log file.
    /// </summary>
    public static string ToHeading()
    {
      return "DateCreated,Method,ClientId,Succeeded";
    }

    /// <summary>
    /// ToString override
    /// </summary>
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.AppendFormat("{0:G},", Date.ToLocalTime());
      sb.AppendFormat("{0},", Method);
      sb.AppendFormat("{0},", ClientId);
      sb.AppendFormat("{0}", Succeeded);
      return sb.ToString();
    }
  }
  
}
