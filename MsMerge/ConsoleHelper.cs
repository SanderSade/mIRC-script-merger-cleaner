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
			throw new NotImplementedException();
		}
	}
}