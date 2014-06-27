using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using FolderComRegistrar;

namespace FolderComRegisterar
{
	public class LibraryRegistrar
	{
		private readonly string[] _extentions;
		private readonly string _libraryDir;

		public LibraryRegistrar(string folder, string[] extentions)
		{
			_libraryDir = folder;
			_extentions = extentions;
		}

		public void StartRgisteringLibraries()
		{
			new Thread(RgitrarThread).Start();
		}

		private void RgitrarThread()
		{
			var files = new List<string>();
			foreach (string extention in _extentions)
			{
				files.AddRange(Directory.GetFiles(_libraryDir, extention.Trim()));
			}
			int count = files.Count;
			if (count > 0)
			{
				for (int index = 0; index < count; index++)
				{
					ProcessFile(files[index], index, count);
				}
				Output.WriteInfo("");
				Output.WriteInfo("");
				Output.WriteInfo("");
				Output.WriteInfo("");
				Output.DrawProgressBar(count, count);
				Output.WriteInfo("");
			}
			else
			{
				Output.WriteError("No files found!");
			}
			
		}

		private void ProcessFile(string file, int index, int count)
		{
			string fileName = Path.GetFileName(file);
			Output.SaveCursorPostion();
			if (RegisterAsNativeComObject(file))
			{
				Output.WriteInfo("Regestering COM library {0}", fileName);
				Output.WriteInfo("");
				Output.WriteInfo("");
				Output.WriteInfo("");
			}
			else if (RegisterAsDotNetComObject(file))
			{

				Output.WriteError("{0} is not COM library",
					fileName);

				Output.WriteInfo("{0} successfully registed as .NET Assembly",
					fileName);
				Output.WriteInfo("");
				Output.WriteInfo("");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Output.WriteInfo("");
				Output.WriteInfo("");
				Output.WriteInfo("");
				Output.WriteInfo("{0} is not a .NET Assembly", fileName);
			}
			Output.DrawProgressBar(index, count);
			Output.WriteInfo("");
			Output.RestoreCursorPosition();
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		private bool RegisterAsNativeComObject(string filename)
		{
			IntPtr hDll = NativeMethods.LoadLibrary(filename);

			if (hDll != IntPtr.Zero)
			{
				try
				{
					IntPtr pDllRegisterServer = NativeMethods.GetProcAddress(hDll, "DllRegisterServer");

					if (pDllRegisterServer != IntPtr.Zero)
					{
						var reg =
							(NativeMethods.DllFuncServer)
								Marshal.GetDelegateForFunctionPointer(pDllRegisterServer,
									typeof (NativeMethods.DllFuncServer));


						return reg() == NativeMethods.S_OK;
					}
					return false;
				}
				finally
				{
					NativeMethods.FreeLibrary(hDll);
				}
			}

			return false;
		}

		private bool RegisterAsDotNetComObject(string filePath)
		{
			try
			{
				Assembly assembly = Assembly.LoadFrom(filePath);
				var regSvc = new RegistrationServices();

				return regSvc.RegisterAssembly(assembly, AssemblyRegistrationFlags.SetCodeBase);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}