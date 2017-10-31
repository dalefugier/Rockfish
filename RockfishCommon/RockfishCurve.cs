using System.Runtime.Serialization;
using Rhino.Geometry;

namespace RockfishCommon
{
  [DataContract]
  public class RockfishCurve
  {
    public RockfishCurve(Curve curve)
    {
      Curve = curve;
    }

    public Curve Curve { get; private set; }

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
      set => Curve = Curve.ToGeometryBase(value) as Curve;
    }

  }
}
