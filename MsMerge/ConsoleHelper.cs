using System;

namespace MsMerge
{
	internal static class ConsoleHelper
	{
		/// <summary>
		/// Show error and exit
		/// </summary>
		internal static void ShowError(string message, int exitCode)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine();
			Console.WriteLine($"[Error] {message}");
			Console.WriteLine();
			Console.ForegroundColor = oldColor;

			Environment.Exit(exitCode);
		}

		/// <summary>
		/// Show warning
		/// </summary>
		/// <param name="message"></param>
		internal static void Warn(string message)
		{
			var oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"[Warning] {message}");
			Console.ForegroundColor = oldColor;
			Program.Warnings++;
		}


		internal static void ShowHelp()
		{
			Console.WriteLine();
			Console.WriteLine("mIRC script merger & cleaner");
			Console.WriteLine("\thttps://github.com/SanderSade/mIRC-script-merger-cleaner");
			Console.WriteLine();
			Console.WriteLine("Use (optional arguments are in parentheses []):");
			Console.WriteLine("\tMsMerge.exe /c:<configuration file>");
			Console.WriteLine("\tMsMerge.exe /output:<output file> [/localaliases] [/minify] [/d] [/stripComments:s|m|a] <infile1> <infile2> <infileN>");
			Console.WriteLine();
			Console.WriteLine("/c, /configuration: specify the JSON configuration file for MsMerge. Can have full path. See the configuration details at the URL above.");
			Console.WriteLine("/o, /output: full path or local filename of the output. If the file exists, it will be renamed to include date & time.");
			Console.WriteLine("/l, /localaliases: mark all aliases as local (-l). Defaults to false if omitted.");
			Console.WriteLine("/m, /minify: remove space and tab characters from start and end of the line, remove empty lines. Does not remove comments, use /s for that. Defaults to false if omitted.");
			Console.WriteLine("/d, /debugRemoval: remove everything between //DEBUG ON and //DEBUG OFF, including those comments. Note that //DEBUG directives must be at the start of the line (spaces/tabs before are OK). Defaults to false if omitted.");
			Console.WriteLine("/s, /stripComments: remove comments. Use a (/s:a) to remove all comments, m to remove multi-line /* */ comments and s to remove single-line (;comment, //comment) comments.");
			Console.WriteLine("All command-line arguments that don't have key flags / or - are considered to be input files. Use double quotation marks (\") if path or filename has spaces. The number of input files is not limited.");
			Console.WriteLine();
			Console.WriteLine("It is strongly recommended to use configuration file over arguments, as the former allows to specify individual settings for every input file!");
			Console.WriteLine();

			Console.WriteLine("Examples:");
			Console.WriteLine("\tMsMerge.exe /c:\"c:\\dev\\myScript\\release-configuration.json\"");
			Console.WriteLine("\tMsMerge.exe /o:c:\\dev\\myScript\\release.mrc /localaliases /minify /d /stripComments:a \"c:\\dev\\myScript\\intro.txt\" \"c:\\dev\\myScript\\main.mrc\" \"c:\\dev\\myScript\\localaliases.mrc\" \"c:\\dev\\myScript\\events.mrc\"");
			Console.WriteLine();
		}
	}
}