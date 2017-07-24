using System;
using MsMerge.Application;
using MsMerge.Dto;

namespace MsMerge
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			var configuration = new ConfigurationParser().Parse(args);

			var merger = new ScriptMerger(configuration);
			merger.Merge();
#if DEBUG
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
#endif
		}


		private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
		{
			ConsoleHelper.ShowError($"{(eventArgs.ExceptionObject as Exception)?.Message}\r\n\r\nFull error message:\r\n{eventArgs.ExceptionObject}", 99);

			Environment.Exit(99);
		}
	}
}