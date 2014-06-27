using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

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
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
				DrawProgressBar(count, count);
				Console.WriteLine();
			}
			else
			{
				Console.WriteLine(MakeConsoleString("No files found!"));
			}
			
		}

		private void ProcessFile(string file, int index, int count)
		{
			string fileName = Path.GetFileName(file);
			int left = Console.CursorLeft;
			int top = Console.CursorTop;
			if (RegisterAsNativeComObject(file))
			{
				Console.WriteLine(MakeConsoleString("Regestering COM library {0}", fileName));
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
			}
			else if (RegisterAsDotNetComObject(file))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write(MakeConsoleString("{0} is not COM library",
					fileName));
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(MakeConsoleString("{0} successfully registed as .NET Assembly",
					fileName));
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString(""));
				Console.WriteLine(MakeConsoleString("{0} is not a .NET Assembly", fileName));
			}
			DrawProgressBar(index, count);
			Console.WriteLine(MakeConsoleString(""));
			Console.CursorLeft = left;
			Console.CursorTop = top;
			Console.ResetColor();
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

		private string MakeConsoleString(string format, params object[] args)
		{
			string text = args.Length > 0 ? String.Format(format, args) : format;
			if (text.Length < Console.BufferWidth)
			{
				int spacesToAdd = Console.BufferWidth - text.Length -1;
				for (int i = 0; i < spacesToAdd; i++)
				{
					text += " ";
				}
			}
			return text;
		}

		private void DrawProgressBar(int complete, int maxVal, int barSize = 40, char progressCharacter = '#')
		{
			Console.CursorVisible = false;
			
			decimal perc = complete/(decimal) maxVal;
			var chars = (int) Math.Floor(perc/(1/(decimal) barSize));
			string p1 = String.Empty, p2 = String.Empty;

			for (int i = 0; i < chars; i++) p1 += progressCharacter;
			for (int i = 0; i < barSize - chars; i++) p2 += progressCharacter;

			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(p1);
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.Write(p2);

			Console.ResetColor();
			Console.WriteLine(" {0}%", (perc*100).ToString("N2"));

			
		}
	}
}