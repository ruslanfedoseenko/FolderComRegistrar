using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderComRegistrar
{
	internal class Output
	{
		private static int m_cLeft, m_cTop;

		public static void Init()
		{
			Console.CancelKeyPress += Console_CancelKeyPress;
		}

		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Console.Clear();
			Console.ResetColor();
		}

		public static void SaveCursorPostion()
		{
			m_cLeft = Console.CursorLeft;
			m_cTop = Console.CursorTop;
		}

		public static void RestoreCursorPosition()
		{
			Console.CursorLeft = m_cLeft;
			Console.CursorTop = m_cTop;
		}

		public static void WriteError(string format, params object[] args)
		{
			Console.ResetColor();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(MakeConsoleString(format, args));
		}

		public static void WriteInfo(string format, params object[] args)
		{
			Console.ResetColor();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(MakeConsoleString(format, args));
		}

		public static void WriteWarning(string format, params object[] args)
		{
			Console.ResetColor();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(MakeConsoleString(format, args));
		}

		private static string MakeConsoleString(string format, params object[] args)
		{
			string text = args.Length > 0 ? String.Format(format, args) : format;
			if (text.Length < Console.BufferWidth)
			{
				int spacesToAdd = Console.BufferWidth - text.Length - 1;
				for (int i = 0; i < spacesToAdd; i++)
				{
					text += " ";
				}
			}
			return text;
		}

		public static void DrawProgressBar(int complete, int maxVal, int barSize = 40, char progressCharacter = '#')
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
