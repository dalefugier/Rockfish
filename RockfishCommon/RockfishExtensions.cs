using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rhino.Geometry;

namespace RockfishCommon
{
  [DataContract]
  public class RockfishGeometry
  {
    public RockfishGeometry(GeometryBase src)
    {
      Geometry = src;
    }

    public GeometryBase Geometry { get; private set; }


  }

  /// <summary>
  /// Extension methods
  /// </summary>
  public static class RockfishExtensions
  {
    public static byte[] ToBytes(this GeometryBase src)
    {
      try
      {
        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
          formatter.Serialize(stream, src);
          return stream.ToArray();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return new byte[0];
    }

    public static GeometryBase ToGeometryBase(this GeometryBase src, byte[] bytes)
    {
      try
      {
        using (var stream = new MemoryStream())
        {
          var formatter = new BinaryFormatter();
          stream.Write(bytes, 0, bytes.Length);
          stream.Seek(0, SeekOrigin.Begin);
          return formatter.Deserialize(stream) as GeometryBase;;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      return null;
    }

  }
}
