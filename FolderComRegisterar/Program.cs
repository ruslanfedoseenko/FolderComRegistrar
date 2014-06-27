using System;
using System.IO;
using FolderComRegistrar;

namespace FolderComRegisterar
{
    class Program
    {
        static void Main(string[] args)
        {
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionEventHandler;
			Output.Init();
            var dir = Directory.GetCurrentDirectory();
	        string[] extentions = {"*.dll","*.ocx"};
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
					Output.WriteInfo("Folder {0} doesnot exists!",dir);
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

		static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
		{
			Output.WriteError(e.ExceptionObject.ToString());
		}

	    private static void PrintUsage()
	    {
			Output.WriteInfo("Usage: {0} [options]", AppDomain.CurrentDomain.FriendlyName);
			Output.WriteInfo("Options:");
			Output.WriteInfo("    -d : Directory to scan for COM objects. Default: Working dirrectory of Application.");
			Output.WriteInfo("    -e : Patterns which will be used for searching of COM objects. Default: \"*.ocx,*.dll\"");
			Output.WriteInfo("    -h : Displays this message.");
	    }
    }
}
