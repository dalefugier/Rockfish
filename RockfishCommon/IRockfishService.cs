using System;
using System.ServiceModel;

namespace RockfishCommon
{
  [ServiceContract]
  public interface IRockfishService
  {
    /// <summary>
    /// Simple test to see if the RockFish service is operational
    /// </summary>
    [OperationContract]
    string Echo(string str);

    [OperationContract]
    Guid AddCurve(RockfishGeometry curve);

    [OperationContract]
    RockfishGeometry[] IntersectBreps(RockfishGeometry brep0, RockfishGeometry brep1, double tolerance);
  }
}
