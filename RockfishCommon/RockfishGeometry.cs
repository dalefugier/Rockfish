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
  ///  of RhinoCommon geometry object.
  /// </summary>
  [DataContract]
  public class RockfishGeometry
  {
    private GeometryBase Geometry { get; set; }

    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="src">The RhinoCommon geometry object.</param>
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
    /// Data contract member
    /// </summary>
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
