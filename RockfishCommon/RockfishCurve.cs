using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Rhino.Geometry;

namespace RockfishCommon
{
  [DataContract]
  public class RockfishCurve
  {
    public RockfishCurve(Rhino.Geometry.Curve curve)
    {
      Curve = curve;
    }

    public Rhino.Geometry.Curve Curve { get; private set; }

    //[DataMember]
    //public byte[] Data
    //{
    //  get
    //  {
    //    var formatter = new BinaryFormatter();
    //    using (var stream = new MemoryStream())
    //    {
    //      formatter.Serialize(stream, m_curve);
    //      return stream.ToArray();
    //    }
    //  }
    //  set
    //  {
    //    using (var stream = new MemoryStream())
    //    {
    //      var formatter = new BinaryFormatter();
    //      stream.Write(value, 0, value.Length);
    //      stream.Seek(0, SeekOrigin.Begin);
    //      Curve = formatter.Deserialize(stream) as Rhino.Geometry.Curve;
    //    }
    //  }
    //}

    [DataMember]
    public byte[] Data
    {
      get => Curve.ToBytes();
      set => Curve = Curve.ToGeometryBase(value) as Rhino.Geometry.Curve;
    }

  }
}
