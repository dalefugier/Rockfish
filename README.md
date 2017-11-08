# Rockfish
The Rockfish sample project demonstrates how one *might* set up a [RhinoCommon](http://developer.rhino3d.com/guides/rhinocommon/) plug-in to host a [Windows Communication Foundation](https://docs.microsoft.com/en-us/dotnet/framework/wcf/whats-wcf) (WCF) service.

### Prerequisites

To build the Rockfish solution, you will need the following:

- [Visual Studio](https://www.visualstudio.com/) - Any of the free version of Microsoft Visual Studio, that can build plug-ins for Rhino, will work. 
- [Rhino](http://www.rhino3d.com/) - The project current targets Rhino 5. But with some minor effort, the project could easily be refactored to work with Rhino 6.

### Overview

The Rockfish sample solution contains the following projects:

**RockfishServer** - This project builds a RhinoCommon plug-in that hosts a WCF Service. The service current uses basic HTTP binding, but it has provisions for named pipes for ease of testing. The service contract has four simple operations that can be called from client systems. The project has a single ```RockfishServer``` command that starts and stops the service. 

Note, to access the service remotely, you will need to open ```TCP Port 8000``` on any firewall software running on the system. And in order for the service to listen on ```TCP Port 8000```, Rhino will need to be launched "as Administrator."

**RockfishClient** - This project also builds a RhinoCommon plug-in, and it consumes the service provided by RockfishServer. The plug-in provides five simple commands:

- ```RF_SetServer``` - Allows you to specify the Rockfish server to be used by the plug-in.
- ```RF_Echo``` - Tests connectivity with the server by sending a string and then printing the server's response on the command line.
- ```RF_PolylineFromPoints``` - Selects points objects and then sends their location to the server. The server creates a polyline curve from the points and sends it back.
- ```RF_IntersectBreps``` - Selects two Brep objects and then sends them to the server. The server calculates the intersection curves and return them back.
- ```RF_CreateMeshFromBrep``` - Selects one or more Brep objects and then sends them to the server. The server generates meshes from the Brep and return them back.

**RockfishCommon** - This project creates a .NET assembly that references RhinoCommon. This assembly provides common classes that are shared between the RockfishServer and RockfishClient projects. The classes and interfaces of interest are:

- ```IRockfishServer``` - This is the [WCF Service Contract](https://docs.microsoft.com/en-us/dotnet/framework/wcf/designing-service-contracts) that is implemented by RockfishServer.
- ```RockfishChannel``` This class is responsible with opening a communication channel with a Rockfish server and calling function in the server's ```IRockfishServer``` implementation.
- ```RockfishGeometry``` - This class defines a [WCF Data Contract](https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/using-data-contracts). The class also handles the binary serialization and de-serialization ```Rhino.Geometry.GeometryBase``` inherited objects.

**RockfishConsole** - This project creates a .NET console application. The project references the [Rhino3dmIO](https://www.nuget.org/packages/Rhino3dmIO.dll-x64-Windows/) NuGet package which allows .NET applications to read and write Rhino's ```.3dm``` file format. When built, the application will read Breps from ```.3dm``` files and the send them to a Rockfish server. The server will, in turn, return mesh objects which are written to a new ```.3dm``` file. Here is the command line syntax:

```C:\> RockfishConsole <host_name> <file_name>```

### Notes

The sample does not address concurrency. Any additions that provide access to the one and only Rhino document will need to do this.



