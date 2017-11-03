using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace RockfishCommon
{
  /// <summary>
  /// Class that handles the serialization and deserialization
  /// of RhinoCommon GeometryBase object.
  /// </summary>
  [DataContract]
  public class RockfishGeometry
  {
    private GeometryBase Geometry { get; set; }

    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="src">The RhinoCommon GeometryBase object.</param>
    public RockfishGeometry(GeometryBase src)
    {
      Geometry = src;
    }

    /// <summary>
    /// Gets the curve if this reference geometry is one.
    /// </summary>
    public Curve Curve => Geometry as Curve;
    
    /// <summary>
    /// Gets the Brep if this reference geometry is one.
    /// </summary>
    public Brep Brep => Geometry as Brep;

    /// <summary>
    /// Gets the extrusion if this reference geometry is one.
    /// </summary>
    public Extrusion Extrusion => Geometry as Extrusion;

    /// <summary>
    /// Gets the mesh if this reference geometry is one.
    /// </summary>
    public Mesh Mesh => Geometry as Mesh;

    /// <summary>
    /// Gets the object type.
    /// </summary>
    public ObjectType ObjectType => Geometry.ObjectType;

    /// <summary>
    /// The GeometryBase member in byte array form.
    /// </summary>
    [DataMember]
    public byte[] Data
    {
      get => ToBytes(Geometry);
      set => Geometry = ToGeometryBase(value);
    }

    /// <summary>
    /// Called during serialization.
    /// Converts an object that inherits from GeometryBase
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
        Console.WriteLine(e.Message);
      }

      return rc;
    }

    /// <summary>
    /// Called during de-serialization.
    /// Creates an object that inherits from GeometryBase
    /// from an array of bytes.
    /// </summary>
    /// <param name="bytes">The array of bytes.</param>
    /// <returns>The geometry if successful.</returns>
    public static GeometryBase ToGeometryBase(byte[] bytes)
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
        Console.WriteLine(e.Message);
      }

      return rc;
    }
  }
}
