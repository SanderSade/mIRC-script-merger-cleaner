# mIRC script merger & cleaner


MsMerge is a program to help with mIRC script development and releasing.

## Features

- **Merge multiple script files to one file for releasing**

During the development, it is easier to have multiple files open, compared to jumping up and down in multi-thousand lines behemoth. 

Using MsMerge configuration, you can specify how every individual file is handled - for example, leave introduction and changelog as-is, but minify and remove comments from script files.

- **Minification & cleaning**

Minify script before releasing, removing formatting and empty lines. This both lowers the download size and memory footprint of a script. 

Note that unlike JS minifiers do, MsMerge does **not** rename aliases and variables - pretty much all non-trivial mIRC scripts use dynamic variables, and there are scripts that rely on other scripts having variables and aliases with specific names.

As a separate option, it is possible to remove comments. MsMerge supports removing all comments, just removing single-line (; or //) or multi-line (/**/) comments. This can be set per-file.


- **Help with debugging statements**

Surround debug statements with //DEBUG ON .. //DEBUG OFF for development. MsMerge can strip the statements and everything between them for releasing. 

In essence, this is comparable to [preprocessor directives](http://www.cprogramming.com/reference/preprocessor/) of C language family, `#ifdef DEBUG... #endif` I.e.
```
//DEBUG ON
on *:connect: {
  doSomethingForDevelopment
  ...
}

alias doSomethingForDevelopment {
...
}
//DEBUG OFF
```
As everything between DEBUG ON and DEBUG OFF gets removed, connect event handler and alias doSomethingForDevelopment will be removed from release version of the script, all reducing the script size, not having to use hacky %isDebug = 1 approach and ensuring that development-only code does not end up in release version.

- **Mark aliases local**

Marking aliases as local (-l) ensures that other scripts cannot access your aliases, therefore removing the chance of alias collision (multiple scripts having alias with the same name). This can be set per-file basis using the configuration file option.


## Configuration

MsMerge can be used with configuration file (JSON) or command-line variables. It is recommended to use the configuration file, as that way it is possible to set individual options for every source file.

### Configuration file

Configuration file is a simple text file in JSON format. Use the configuration file from command-line:

    MsMerge.exe -c:"c:\dev\scripts\myScript\msmerge.release.json


MsMerge includes schema file ([MsMerge.schema.json](https://raw.githubusercontent.com/SanderSade/mIRC-script-merger-cleaner/master/MsMerge/Schema/MsMerge.schema.json)). Using a compatible editor, you can get help (suggestions and descriptions) when creating a new configuration file. Editors supporting JSON schema functionality include Visual Studio, Visual Studio Code and many more. You can also use online [JSON schema validator](http://www.jsonschemavalidator.net/) to edit the configuration file.

#### Example configuration file

[MsMerge.example.json](https://github.com/SanderSade/mIRC-script-merger-cleaner/blob/master/MsMerge/Schema/MsMerge.example.json). Note that you have to escape backslashes (\\) in file names with double backslashes (\\\\).

```
{
  "$schema": "MsMerge.schema.json",
  "input": [
    {
      "script": "c:\\dev\\myScript\\intro.txt",
      "commentMode": "Off"
    },
    {
      "script": "c:\\dev\\myScript\\main.mrc",
      "commentMode": "SingleLine",
      "minify": true,
      "stripDebug": true,
      "localAliases": true
    },
    {
      "script": "c:\\dev\\myScript\\globalAliases.mrc",
      "commentMode": "All",
      "minify": true,
      "stripDebug": true,
      "localAliases": false
    },
    {
      "script": "c:\\dev\\myScript\\events.mrc",
      "commentMode": "MultiLine",
      "minify": true,
      "stripDebug": true,
      "localAliases": true
    }
    ],
  "output": "c:\\dev\\myScript\\Release\\myScript.mrc"
}
```

### Command-line options

```
Use (optional arguments are in parentheses []):
  MsMerge.exe /c:<configuration file>
  MsMerge.exe /output:<output file> [/localaliases] [/minify] [/d] [/stripComments:s|m|a] <infile1> <infile2> <infileN>

/c, /configuration: specify the JSON configuration file for MsMerge. Can have full path. See the configuration details at the URL above.
/o, /output: full path or local filename of the output. If the file exists, it will be renamed to include date & time.
/l, /localaliases: mark all aliases as local (-l). Defaults to false if omitted.
/m, /minify: remove space and tab characters from start and end of the line, remove empty lines. Does not remove comments, use /s for that. Defaults to false if omitted.
/d, /debugRemoval: remove everything between //DEBUG ON and //DEBUG OFF, including those comments. Note that //DEBUG directives must be at the start of the line (spaces/tabs before are OK). Defaults to false if omitted.
/s, /stripComments: remove comments. Use a (/s:a) to remove all comments, m to remove multi-line /* */ comments and s to remove single-line (;comment, //comment) comments.
All command-line arguments that don't have key flags / or - are considered to be input files. Use double quotation marks (") if path or filename has spaces. The number of input files is not limited.


Examples:
  MsMerge.exe /c:"c:\dev\myScript\release-configuration.json"
  MsMerge.exe /o:c:\dev\myScript\release.mrc /localaliases /minify /d /stripComments:a "c:\dev\myScript\intro.txt" "c:\dev\myScript\main.mrc" "c:\dev\myScript\localaliases.mrc" "c:\dev\myScript\events.mrc"
```
