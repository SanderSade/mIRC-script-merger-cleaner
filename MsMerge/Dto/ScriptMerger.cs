﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MsMerge.Application;

namespace MsMerge.Dto
{
	internal sealed class ScriptMerger
	{
		private readonly Configuration _configuration;
		private readonly StringBuilder _output;
		private long _totalSize;


		/// <inheritdoc />
		internal ScriptMerger(Configuration configuration)
		{
			_configuration = configuration;
			_output = new StringBuilder();
		}


		internal void Merge()
		{
			_output.Clear();

			foreach (var inFile in _configuration.Infiles)
			{
				AddFile(inFile);
			}

			ValidateOutputFile();

			Console.WriteLine($"Writing output file: {_configuration.OutFile}");
			File.WriteAllText(_configuration.OutFile, _output.ToString(), Encoding.UTF8);
			var outSize = new FileInfo(_configuration.OutFile).Length;

			Console.WriteLine(
				$"Total input size (bytes): {_totalSize}, output: {outSize}. Reduction was {_totalSize - outSize} bytes ({100 - Math.Round(outSize * 100f / _totalSize, 2)}%)");
		}


		private void ValidateOutputFile()
		{
			if (File.Exists(_configuration.OutFile))
			{
				var fi = new FileInfo(_configuration.OutFile);
				var oldFileCopy = Path.ChangeExtension(_configuration.OutFile,
					$"{fi.LastWriteTime:yyyyMMdd.HHmmss}{Path.GetExtension(_configuration.OutFile)}");

				if (File.Exists(oldFileCopy))
					oldFileCopy = Path.ChangeExtension(oldFileCopy,
						$"-{DateTimeOffset.Now:yyyyMMdd.HHmmss}{Path.GetExtension(_configuration.OutFile)}");

				ConsoleHelper.Warn($"Output file \"{_configuration.OutFile}\" exists. Old file will be renamed to {oldFileCopy}");

				Debug.Assert(oldFileCopy != null, "oldFileCopy != null");
				Debug.Assert(_configuration.OutFile != null, "_configuration.OutFile != null");
				File.Move(_configuration.OutFile, oldFileCopy);
			}
		}


		private void AddFile(InFile inFile)
		{
			Console.WriteLine($"Processing {inFile.Filename}");
			_totalSize += new FileInfo(inFile.Filename).Length;

			var lines = File.ReadAllLines(inFile.Filename);
			var debugLine = false;
			var multiLineComment = false;

			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				var trimmedLine = line.Trim();

				if (!multiLineComment && inFile.Minify && string.IsNullOrWhiteSpace(trimmedLine))
					continue;

				if (inFile.Minify)
					line = trimmedLine;

				/*
				* https://en.wikichip.org/wiki/mirc/introduction#Multi-line_Comments
				* Text may touch the opening /* on the right; however, /* must start the line
				* The closing * / must be on a line of its own
				*/

				//check for multi-line comment start/end
				//we need to do this even if we aren't stripping the comments, as we don't want to trim multi-line comments
				if (string.Compare(trimmedLine, "*/", StringComparison.Ordinal) == 0)
				{
					if (!multiLineComment)
					{
						ConsoleHelper.Warn($"Multi-line comment (/* */) ends but was never started. File {inFile.Filename}, line {i + 1}");
						if (inFile.CommentMode == CommentMode.All || inFile.CommentMode == CommentMode.MultiLine)
							continue;
					}

					multiLineComment = false;

					if (inFile.CommentMode == CommentMode.All || inFile.CommentMode == CommentMode.MultiLine)
						continue;
				}

				if (multiLineComment && (inFile.CommentMode == CommentMode.All || inFile.CommentMode == CommentMode.MultiLine))
					continue;

				if (trimmedLine.StartsWith("/*", StringComparison.Ordinal))
				{
					multiLineComment = true;
					if (inFile.CommentMode == CommentMode.All || inFile.CommentMode == CommentMode.MultiLine)
						continue;
				}

				//add multiline comments as-is even if we're minifying
				if (multiLineComment)
				{
					_output.AppendLine(lines[i]);
					continue;
				}

				//single-line comment or DEBUG ON/OFF  (;"// ")
				if (trimmedLine.StartsWith(";", StringComparison.Ordinal) || trimmedLine.StartsWith("//", StringComparison.Ordinal))
				{
					var markerRemoved = trimmedLine.TrimStart(';', '/');
					if (inFile.StripDebug)
					{
						if (markerRemoved.StartsWith("DEBUG OFF", StringComparison.Ordinal))
						{
							if (!debugLine)
							{
								ConsoleHelper.Warn($"DEBUG OFF encountered when there is no matching DEBUG ON. File {inFile.Filename}, line {i + 1}");
								continue;
							}

							debugLine = false;
							continue;
						}

						if (markerRemoved.StartsWith("DEBUG ON", StringComparison.Ordinal))
						{
							debugLine = true;
							continue;
						}
					}

					if (inFile.CommentMode == CommentMode.All || inFile.CommentMode == CommentMode.SingleLine)
						continue;
				}

				if (inFile.StripDebug && debugLine)
					continue;

				//check if it is alias, and if it is, add -l if needed
				if (inFile.LocalAliases && trimmedLine.StartsWith("alias ", StringComparison.OrdinalIgnoreCase))
				{
					//this is cumbersome way to achieve this, but I don't want to mess with double spaces and such
					var split = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (split.Length >= 2)
					{
						if (string.Compare(split[1], "-l", StringComparison.OrdinalIgnoreCase) != 0)
						{
							line = Regex.Replace(line, "alias ", "alias -l ", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
						}
					}
				}

				if (trimmedLine.Contains("  "))
					ConsoleHelper.Warn($"Possibly unintended double/multi spaces. File {inFile.Filename}, line {i + 1}: {line}");

				_output.AppendLine(line);
			}

			if (multiLineComment)
				ConsoleHelper.Warn($"Multi-line comment (/* */) starts but is never closed. File {inFile.Filename}");

			if (debugLine)
				ConsoleHelper.Warn($"DEBUG ON begins but is never ended. File {inFile.Filename}");
		}
	}
}