using System.Runtime.Serialization;
using Rhino.Geometry;

namespace RockfishCommon
{
  [DataContract]
  public class RockfishPoint
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="src">The RhinoCommon Point3d object.</param>
    public RockfishPoint(Point3d src)
    {
      X = src.X;
      Y = src.Y;
      Z = src.Z;
    }

    /// <summary>
    /// Gets or sets the X (first) coordinate of this point.
    /// </summary>
    [DataMember]
    public double X { get; private set; }

    /// <summary>
    /// Gets or sets the Y (second) coordinate of this point.
    /// </summary>
    [DataMember]
    public double Y { get; private set; }

    /// <summary>
    /// Gets or sets the Z (third) coordinate of this point.
    /// </summary>
    [DataMember]
    public double Z { get; private set; }

    /// <summary>
    /// Get a RhinoCommon Point3d object
    /// </summary>
    public Point3d ToPoint3D => new Point3d(X, Y, Z);
  }

}