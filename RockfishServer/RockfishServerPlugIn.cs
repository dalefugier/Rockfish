namespace RockfishServer
{
  /// <summary>
  /// RockfishServerPlugIn
  /// </summary>
  public class RockfishServerPlugIn : Rhino.PlugIns.PlugIn
  {
    public RockfishServerPlugIn()
    {
      Instance = this;
    }

    /// <summary>
    /// Gets the only instance of the RockfishServerPlugIn plug-in.
    /// </summary>
    public static RockfishServerPlugIn Instance
    {
      get; private set;
    }

    // You can override methods here to change the plug-in behavior on
    // loading and shut down, add options pages to the Rhino _Option command
    // and maintain plug-in wide options in a document.
  }
}