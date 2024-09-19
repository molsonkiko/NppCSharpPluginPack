# NppCSharpPluginPack

[![Continuous Integration](https://github.com/molsonkiko/NppCSharpPluginPack/actions/workflows/CI_build.yml/badge.svg)](https://github.com/molsonkiko/NppCSharpPluginPack/actions/workflows/CI_build.yml)

[![License](http://img.shields.io/badge/License-Apache_2-red.svg?style=flat)](http://www.apache.org/licenses/LICENSE-2.0)

This is a template for Notepad++ plugins written in C#. The vast majority of the code (certainly all the parts that were really hard to implement) come from [kbilsted's now-archived `NotepadPlusPlusPluginPack.Net`](https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net), with some significant changes, including:

1. A simpler architecture in which the [PluginInfrastructure folder](/NppCSharpPluginPack/PluginInfrastructure/) is side-by-side with the rest of the plugin's code, rather than in a separate directory tree.
2. An example of a [non-docking (pop-up) dialog](/docs/README.md#about-form) opened from the plugin menu, a [docking form](/docs/README.md#selections-remembering-form), and a [non-docking non-pop-up form](/docs/README.md#dark-mode-test-form).
3. A [settings form](/docs/README.md#settings-form) that makes it easy for the maintainer to add, remove or edit settings.
4. A template for [automated tests](/docs/README.md#running-tests) that can be run inside Notepad++, including a template for user interface tests that automatically test forms.

If you have any issues, see if [updating to the latest release](https://github.com/molsonkiko/NppCSharpPluginPack/releases) helps, and then feel free to raise an [issue](https://github.com/molsonkiko/NppCSharpPluginPack/issues) on GitHub. Please be sure to include diagnostic information about your system, Notepad++ version, and plugin version (go to `?->Debug Info...` from the Notepad++ main menu).

[Read the docs.](/docs/README.md)

[Read information about the plugin architecture (*some of this may be out of date*)](/PluginPackArchitecture.md)

[View past changes.](/CHANGELOG.md)

## Using the template ##

1. Download the source code in one of the following ways:
    - clone the repository
    - download the source code for the repo, then create a git repo inside
    - fork this repo
    - download the source code for a release (see the [Releases page](https://github.com/molsonkiko/NppCSharpPluginPack/releases))
2. Open [NppCSharpPluginPack/NppCSharpPluginPack.sln](https://github.com/molsonkiko/NppCSharpPluginPack/blob/main/NppCSharpPluginPack/NppCSharpPluginPack.sln) in Visual Studio 2022
3. Configure the security settings of `%ProgramFiles%/Notepad++/plugins` so that Visual Studio can write to `%ProgramFiles%/Notepad++/plugins/CSharpPluginPack`. I do this in the following way:
    1. Open `%ProgramFiles%/Notepad++` in the File Explorer.
    2. Right-click on the `plugins` folder.
    3. Choose `Properties` from the drop-down menu
    4. Select the `Security` tab.
    5. Click the `Edit...` checkbox
    6. Select `Users` from the `Group or user names` menu, then click the checkbox in the `Full Control` row of the `Allow` column.
    7. Click the `Apply` button.
4. Try building the project using Visual Studio using `x64` build target (this build for 64-bit Notepad++). You will know that the project is working correctly when Notepad++ starts up, and `CSharpPluginPack` appears on the list of plugins.
5. Try changing the assembly version in [NppCSharpPluginPack/Properties/AssemblyInfo.cs](https://github.com/molsonkiko/NppCSharpPluginPack/blob/main/NppCSharpPluginPack/Properties/AssemblyInfo.cs) to `1.0.3.0`.
6. Try building the project again, then select `Plugins->CSharpPluginPack->About` from the Notepad++ dropdown menu. The title of the [about form](/docs/README.md#about-form) should read `NppCSharpPluginPack v1.0.3`. If it does not, your project is not building correctly.
7. Copy the `testfiles` directory of the repo to the `%ProgramFiles%/Notepad++/plugins/CSharpPluginPack` directory.
8. Run the tests. The third line of the test results file should read `No tests failed`. If you see `Tests failed: Performance of something` on the third line, that's because you forgot step 7.
9. To test your project for 32-bit Notepad++, repeat steps 3-8 for `%ProgramFiles (x86)%/Notepad++/plugins` and change your build target to `x86`.

## System Requirements ##

This plugin is verified to work on versions of Notepad++ as old as [v7.3.3](https://notepad-plus-plus.org/downloads/v7.3.3/), but it may have some bugs ([see the CHANGELOG](/CHANGELOG.md) to see the what bugs may still exist).

It has also been tested and verified to work normally on the newest versions of Notepad++, [v8.6.1](https://notepad-plus-plus.org/downloads/v8.6.1/) and [v8.6.2 RC](https://community.notepad-plus-plus.org/topic/25341/notepad-v8-6-2-release-candidate/19).

Every version of the template works on [Windows 10 May 2019 update](https://blogs.windows.com/windowsexperience/2019/05/21/how-to-get-the-windows-10-may-2019-update/) or later, or you must install [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).

## Translating your plugin to another language ##

If you are interested in helping users of your plugin who don't speak English, this plugin pack makes it easy to translate your plugin to other languages beginning in [v0.0.3.7](/CHANGELOG.md#004---unreleased-yyyy-mm-dd).

[`Translator.cs`](https://github.com/molsonkiko/NppCSharpPluginPack/blob/main/NppCSharpPluginPack/Utils/Translator.cs) infers your preferred language and attempts to translate in the following way:
1. It first attempts to use [NPPM_GETNATIVELANGFILENAME](https://npp-user-manual.org/docs/plugin-communication/#2140nppm_getnativelangfilename) (introduced in Notepad++ 8.7) to get the name of the Notepad++ UI language.
2. If step 1 fails, it checks your [Notepad++ `nativeLang.xml` config file](https://npp-user-manual.org/docs/binary-translation/#creating-or-editing-a-translation) (at [XPath path](https://www.w3schools.com/xml/xml_xpath.asp) `/NotepadPlus/Native-Langue/@name`) to determine what language you prefer to use, and sets `lowerEnglishName` to the appropriate value and __skips to step 3__. For example, if this file says `galician`, we will attempt to translate your plugin to `galician`. __If the Notepad++ native language is `english` *or* if your plugin does not have a translation file for the Notepad++ native language, `Translator.cs` then does the following:__
3. If steps 1 and 2 fail, `Translator.cs` looks up your [`current culture`](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.utility/get-culture?view=powershell-7.4) (which can be checked using the `get-culture` command in Powershell).
    * Next, we find the `EnglishName` property of the `current culture` (which can be found by the `$foo = get-culture; $foo.EnglishName` command in Powershell), take the first word, and convert it to lowercase, and set `lowerEnglishName` to this.
        - Example: the `EnglishName` of the `en-us` culture is `English (United States)`, so `lowerEnglishName` is `english`
        - Example: the `EnglishName` of the `it-it` culture is `Italian (Italy)`, so `lowerEnglishName` is `italian`
4. `Translator.cs` then does one of the following:
    - If `lowerEnglishName` is `english`, it does nothing (because the plugin is naturally in English)
    - Otherwise, it looks in the `translation` subdirectory of the `CSharpPluginPack` plugin folder (where `CSharpPluginPack.dll` lives) for a file named `{lowerEnglishName}.json5`
    - __NOTE:__ because the translation files are in a subdirectory of the plugin folder, *translation does not work for versions of Notepad++ older than [version 8](https://notepad-plus-plus.org/downloads/v8/),* since those older versions do not have separate folders for each plugin.
5. If `Translator.cs` found `translation\{lowerEnglishName}.json5`, it attempts to parse the file. If parsing fails, a message box will appear warning the user of this.
    - If no translation file was found, or if parsing failed, the default English will be used.
6. If parsing was successful, `Translator.cs` will use the translation file as described below.

When the user changes their Notepad++ UI language, the plugin will respond to this change as follows (according to the version number of NppCSharpPluginPack):

| CSharpPluginPack version | Translation at startup | Translation *possible* (but slow) as soon as user changes their preference | Translation *guaranteed* when user changes their preference |
|--------------------------|------------------------|----------------------------------------------------------------------------|-------------------------------------------------------------|
|  v0.0.3.7 to 0.0.3.10    |   __YES__              |   __NO__                                                                   |                   __NO__                                    |
|  v0.0.3.11               |   __YES__              |   __YES__                                                                  |                   __NO__                                    |
|  v0.0.3.14 or higher     |   __YES__              |   __YES__                                                                  |   __YES__ (only for Notepad++ 8.7 or higher)                |

To translate your plugin to another language, just look at [`english.json5` in the translations directory of this repo](https://github.com/molsonkiko/NppCSharpPluginPack/blob/main/translation/english.json5) and follow the instructions in that file.

Currently NppCSharpPluginPack has been translated into the following languages:
| Language | First version with translation | Translator(s) | Translator is native speaker?  |
|----------|--------------------------------|---------------|--------------------------------|
| [Spanish](https://github.com/molsonkiko/NppCSharpPluginPack/blob/main/translation/spanish.json5)  |   [v3.7](https://github.com/molsonkiko/NppCSharpPluginPack/commit/cc93a081e1fc1de3c10a953bb0d1ac035326b19e) |  [molsonkiko](https://github.com/molsonkiko)  |  __NO__ |


The following aspects of NppCSharpPluginPack __can__ be translated:
- Forms (including all controls and items in drop-down menus) (see the `TranslateForm` method in `Translator.cs`)
- Items in the NppCSharpPluginPack sub-menu of the Notepad++ Plugins menu (see the `GetTranslatedMenuItem` method in `Translator.cs`)
- The descriptions of settings in the [`CSharpPluginPack.ini` config file and settings form](/docs/README.md#settings-form) (see the `TranslateSettingsDescription` method in `Translator.cs`)
- Message boxes (includes warnings, errors, requests for confirmation) (see the `ShowTranslatedMessageBox` method in `Translator.cs`)

The following aspects of NppCSharpPluginPack __may eventually__ be translated:
- This documentation
- The messages of user-defined exceptions (for example, if your plugin `FooHasNoBarException`, there is currently no way to translate the message that the user sees when this error is thrown and caught)
- Generic modal dialogs (for example, file-opening dialogs, directory selection dialogs)

## Acknowledgments ##

* [Kasper B. Graversen](https://github.com/kbilsted) for creating the [plugin pack](https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net) that this is based on.
* [jokedst](https://github.com/jokedst) for making the [CsvQuery plugin](https://github.com/jokedst/CsvQuery) to which I owe the original ideas behind my settings form and my adaptive styling of forms.
* And of course, Don Ho for creating [Notepad++](https://notepad-plus-plus.org/)!
