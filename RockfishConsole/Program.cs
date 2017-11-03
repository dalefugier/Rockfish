using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Rhino.DocObjects;
using Rhino.FileIO;
using Rhino.Geometry;
using RockfishCommon;

namespace RockfishConsole
{
  class Program
  {
    static int Main(string[] args)
    {
      if (2 != args.Length)
      {
        Console.WriteLine("Usage: RockfishConsole <hostname> <filename>");
        return 1;
      }

      var host_name = LookupHostName(args[0]);
      if (string.IsNullOrEmpty(host_name))
      {
        Console.WriteLine("Unable to lookup host name: \"{0}\".", args[0]);
        return 1;
      }

      var path = Path.GetFullPath(args[1]);
      if (!File.Exists(path))
      {
        Console.WriteLine("File not found: \"{0}\".", path);
        return 1;
      }

      var in_file = File3dm.Read(path);
      if (null == in_file)
      {
        Console.WriteLine("Unable to read file: \"{0}\".", path);
        return 1;
      }

      var breps = new List<Brep>();
      foreach (var obj in in_file.Objects)
      {
        if (obj.Geometry.ObjectType == ObjectType.Brep)
        {
          var brep = obj.Geometry as Brep;
          if (null != brep)
            breps.Add(brep);
        }
        else if (obj.Geometry.ObjectType == ObjectType.Extrusion)
        {
          var extrusion = obj.Geometry as Extrusion;
          var brep = extrusion?.ToBrep(true);
          if (brep != null)
            breps.Add(brep);
        }
      }

      if (0 == breps.Count)
      {
        Console.WriteLine("No Breps found in file:: \"{0}\".", path);
        return 1;
      }

      try
      {
        using (var channel = new RockfishChannel())
        {
          channel.Create(host_name);

          Console.WriteLine(path);
          var filename = Path.GetFileNameWithoutExtension(path);
          Console.WriteLine(filename);

          for (var i = 0; i < breps.Count; i++)
          {
            var in_brep = new RockfishGeometry(breps[i]);
            var out_mesh = channel.CreateMeshFromBrep(in_brep, false);
            if (null != out_mesh?.Mesh)
            {
              var new_filename = $"{filename}_mesh{i}";
              Console.WriteLine(new_filename);
              var out_path = path.Replace(filename, new_filename);
              Console.WriteLine(out_path);

              var out_file = new File3dm();
              out_file.Objects.AddMesh(out_mesh.Mesh);
              out_file.Polish();


              out_file.Write(out_path, 5);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return 1;
      }

      return 0;
    }

    /// <summary>
    /// Resolves a host name to a canonical host name.
    /// </summary>
    public static string LookupHostName(string hostName)
    {
      if (string.IsNullOrEmpty(hostName))
        return null;

      // If the string parses as an IP address, return.
      if (IPAddress.TryParse(hostName, out IPAddress address))
        return hostName;

      try
      {
        var host_entry = Dns.GetHostEntry(hostName);
        return host_entry.HostName;
      }
      catch
      {
        // ignored
      }

      return null;
    }


  }
}
