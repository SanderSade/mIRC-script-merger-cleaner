using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MsMerge.Dto;
using Newtonsoft.Json;

namespace MsMerge.Application
{
	internal class ConfigurationParser
	{
		private Configuration _configuration;


		internal Configuration Parse(string[] args)
		{
			if (args == null || args.Length == 0)
			{
				ConsoleHelper.ShowHelp();
				return null;
			}

			_configuration = new Configuration { Infiles = new List<InFile>() };
			var keys = args.Where(x => x.StartsWith("/") || x.StartsWith("-"));

			var keysClean = keys.Select(x => x.TrimStart('/', '-')).ToArray();

			var confKey = keysClean.FirstOrDefault(x => x.StartsWith("c"));

			if (confKey != null)
			{
				ParseConfigurationFile(confKey);
				return _configuration;
			}

			var globalFileSetting = ParseKeys(keysClean);



			if (string.IsNullOrWhiteSpace(globalFileSetting.Filename))
			{
				ConsoleHelper.ShowError("Output file must be set!", 12);
				return null;
			}

			_configuration.OutFile = globalFileSetting.Filename;

			var infiles = args.Where(x => !x.StartsWith("/") && !x.StartsWith("-")).ToArray();


			if (!infiles.Any())
			{
				ConsoleHelper.ShowError("No input files specified!", 10);
				return null;
			}

			foreach (var file in infiles)
			{
				if (!File.Exists(file))
				{
					ConsoleHelper.ShowError($"Input file \"{file}\" does not exist!", 11);
					return null;
				}


				_configuration.Infiles.Add(new InFile
				{
					CommentMode = globalFileSetting.CommentMode,
					LocalAliases = globalFileSetting.LocalAliases,
					Minify = globalFileSetting.Minify,
					StripDebug = globalFileSetting.StripDebug,
					Filename = file
				});
			}


			return _configuration;
		}


		private static InFile ParseKeys(string[] keysClean)
		{
			var global = new InFile { CommentMode = CommentMode.Off };

			foreach (var key in keysClean)
			{

				switch (key[0])
				{
					case 'd':
						global.StripDebug = true;
						continue;
					case 's':
						global.CommentMode = GetCommentMode(key);
						continue;
					case 'm':
						global.Minify = true;
						continue;
					case 'l':
						global.LocalAliases = true;
						continue;
					case 'o':
						global.Filename = GetValue(key);
						continue;
					default:
						ConsoleHelper.ShowHelp();
						ConsoleHelper.ShowError($"Invalid configuration key: {key}", 8);
						continue;
				}
			}

			return global;
		}


		private static CommentMode GetCommentMode(string key)
		{
			var commentMode = GetValue(key).ToLowerInvariant();
			switch (commentMode)
			{
				case "s":
					return CommentMode.SingleLine;

				case "m":
					return CommentMode.MultiLine;

				case "a":
					return CommentMode.All;

				default:
					ConsoleHelper.ShowError($"Invalid comment mode: {commentMode}. Use s, m, a", 7);
					return CommentMode.Off;
			}
		}



		private static string GetValue(string key)
		{
			if (!key.Contains(":"))
			{
				ConsoleHelper.ShowError($"Key {key} expects value in format /key:value", 5);
				return null;
			}

			var value = key.Substring(key.IndexOf(":", StringComparison.Ordinal) + 1);

			if (string.IsNullOrWhiteSpace(value))
			{
				ConsoleHelper.ShowError($"Key {key} value cannot be empty!", 6);
				return null;
			}

			value = value.Trim('"', ' ').Trim();

			return value;
		}


		private void ParseConfigurationFile(string key)
		{
			var file = GetValue(key);

			if (!File.Exists(file))
				ConsoleHelper.ShowError($"Configuration file \"{file}\" does not exist!", 20);

			_configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));

			foreach (var configurationInfile in _configuration.Infiles)
			{
				if (!File.Exists(configurationInfile.Filename))
					ConsoleHelper.ShowError($"Input file \"{configurationInfile.Filename}\" does not exist!", 11);
			}
		}
	}
}