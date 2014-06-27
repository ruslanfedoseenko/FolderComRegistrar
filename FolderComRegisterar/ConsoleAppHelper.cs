using System;
using System.Collections.Generic;

namespace FolderComRegistrar
{
	class ConsoleAppHelper
	{
		public static Dictionary<String, String> ResolveCmdArgsDictionary(string[] argv)
		{
			Dictionary<String, String> argsMap = new Dictionary<string, string>();
			int i = 0;
			while (i < argv.Length)
			{
				if (argv[i].StartsWith("-"))
				{
					string value = String.Empty;
					if (i + 1 < argv.Length && !argv[i + 1].StartsWith("-"))
					{
						value = argv[i + 1];
					}
					argsMap.Add(argv[i],value);
				}
				i++;
			}
			return argsMap;
		}
	}
}
