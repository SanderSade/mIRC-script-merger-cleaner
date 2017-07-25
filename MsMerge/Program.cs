using System;
using System.Diagnostics;
using System.Linq;
using MsMerge.Application;
using MsMerge.Dto;

namespace MsMerge
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			if (args == null || args.Length == 0 || args.Any(x => x.Contains("?")))
			{
				ConsoleHelper.ShowHelp();
			}
			else
			{
				var sw = Stopwatch.StartNew();
				AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
				var configuration = new ConfigurationParser().Parse(args);

				var merger = new ScriptMerger(configuration);
				merger.Merge();
				sw.Stop();
				Console.WriteLine($"Finished in {sw.Elapsed}{(Warnings > 0 ? " with " + Warnings + " warnings." : ".")}");
			}
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


		public static int Warnings { get; set; }
	}
}