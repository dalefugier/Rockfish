using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RockfishCommon
{
  [DataContract]
  public class RockfishCurve
  {
    private readonly Rhino.Geometry.Curve m_curve;

    public RockfishCurve(Rhino.Geometry.Curve curve)
    {
      m_curve = curve;
    }

    [DataMember]
    public byte[] Curve
    {
      get
      {
        var formatter = new BinaryFormatter();
        var info = new SerializationInfo();
        m_curve.GetObjectData();
      }
      set;
    }
  }
}
