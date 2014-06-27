using System;
using System.IO;
using FolderComRegistrar;

namespace FolderComRegisterar
{
    class Program
    {
        static void Main(string[] args)
        {
			Output.Init();
            var dir = Directory.GetCurrentDirectory();
	        string[] extentions = new[]{"*.dll","*.ocx"};
	        var argsMap = ConsoleAppHelper.ResolveCmdArgsDictionary(args);
	        if (argsMap.ContainsKey("-h"))
	        {
		        PrintUsage();
				return;
	        }
	        if (argsMap.ContainsKey("-d"))
	        {
		        dir = argsMap["-d"];
		        if (!Directory.Exists(dir))
		        {
					Console.WriteLine("{0} doesnot exists!",dir);
					PrintUsage();
			        return;
		        }
				Directory.SetCurrentDirectory(dir);
	        }
	        if (argsMap.ContainsKey("-e"))
	        {
		        extentions = argsMap["-e"].Split(',');
	        }
	        var libReg = new LibraryRegistrar(dir,extentions);
			libReg.StartRgisteringLibraries();
        }

	    private static void PrintUsage()
	    {
			Console.WriteLine("Usage: {0} [options]", AppDomain.CurrentDomain.FriendlyName);
			Console.WriteLine("Options:");
			Console.WriteLine("    -d : Directory to scan for COM objects. Default: Working dirrectory of Application.");
			Console.WriteLine("    -e : Patterns which will be used for searching of COM objects. Default: \"*.ocx,*.dll\"");
			Console.WriteLine("    -h : Displays this message.");
	    }
    }
}
