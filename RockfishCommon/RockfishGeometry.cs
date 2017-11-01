using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rhino;
using Rhino.Geometry;

namespace RockfishCommon
{
  [DataContract]
  public class RockfishGeometry
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="src">The RhinoCommon geometry object.</param>
    public RockfishGeometry(GeometryBase src)
    {
      Geometry = src;
    }

    public GeometryBase Geometry { get; private set; }

    public Curve Curve => Geometry as Curve;
    public Brep Brep => Geometry as Brep;
    public Extrusion Extrusion => Geometry as Extrusion;
    public Mesh Mesh => Geometry as Mesh;

    [DataMember]
    public byte[] Data
    {
      get => ToBytes(Geometry);
      set => Geometry = ToGeometry(value);
    }

    /// <summary>
    /// Converts a object that inherits from Rhino.Geometry.GeometryBase
    /// to an array of bytes.
    /// </summary>
    private static byte[] ToBytes(GeometryBase src)
    {
      var rc = new byte[0];
      if (null == src)
        return rc;

      try
      {
        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
          formatter.Serialize(stream, src);
          rc = stream.ToArray();
        }
      }
      catch (Exception e)
      {
        RhinoApp.WriteLine(e.Message);
      }

      return rc;
    }

    public static GeometryBase ToGeometry(byte[] bytes)
    {
      if (null == bytes || 0 == bytes.Length)
        return null;

      GeometryBase rc = null;
      try
      {
        using (var stream = new MemoryStream())
        {
          var formatter = new BinaryFormatter();
          stream.Write(bytes, 0, bytes.Length);
          stream.Seek(0, SeekOrigin.Begin);
          var geometry = formatter.Deserialize(stream) as GeometryBase;
          if (null != geometry && geometry.IsValid)
            rc = geometry;
        }
      }
      catch (Exception e)
      {
        RhinoApp.WriteLine(e.Message);
      }

      return rc;
    }
  }
}
