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

[Read information about the plugin architecture (some of this may be out of date)](/PluginPackArchitecture.md)

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
5. Try changing the assembly version in [NppCSharpPluginPack/Properties/AssemblyInfo.cs](https://github.com/molsonkiko/NppCSharpPluginPack/blob/main/NppCSharpPluginPack/Properties/AssemblyInfo.cs) to `1.0.3`.
6. Try building the project again, then select `Plugins->CSharpPluginPack->About` from the Notepaad++ dropdown menu. The title of the [about form](/docs/README.md#about-form) should read `NppCSharpPluginPack v1.0.3`. If it does not, your project is not building correctly.
7. Copy the `testfiles` directory of the repo to the `%ProgramFiles%/Notepad++/plugins/CSharpPluginPack` directory.
8. Run the tests. The third line of the test results file should read `No tests failed`. If you see `Tests failed: Performance of something` on the third line, that's because you forgot step 7.
9. To test your project for 32-bit Notepad++, repeat steps 3-8 for `%ProgramFiles (x86)%/Notepad++/plugins` and change your build target to `x86`.

## System Requirements ##

This plugin is verified to work on versions of Notepad++ as old as [v7.3.3](https://notepad-plus-plus.org/downloads/v7.3.3/), but it may have some bugs ([see the CHANGELOG](/CHANGELOG.md) to see the what bugs may still exist).

It has also been tested and verified to work normally on the newest versions of Notepad++, [v8.6.1](https://notepad-plus-plus.org/downloads/v8.6.1/) and [v8.6.2 RC](https://community.notepad-plus-plus.org/topic/25341/notepad-v8-6-2-release-candidate/19).

Every version of the template works on [Windows 10 May 2019 update](https://blogs.windows.com/windowsexperience/2019/05/21/how-to-get-the-windows-10-may-2019-update/) or later, or you must install [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).

## Acknowledgments ##

* [Kasper B. Graversen](https://github.com/kbilsted) for creating the [plugin pack](https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net) that this is based on.
* [jokedst](https://github.com/jokedst) for making the [CsvQuery plugin](https://github.com/jokedst/CsvQuery) to which I owe the original ideas behind my settings form and my adaptive styling of forms.
* And of course, Don Ho for creating [Notepad++](https://notepad-plus-plus.org/)!
