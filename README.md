# NppCSharpPluginPack

[![Continuous Integration](https://github.com/molsonkiko/NppCSharpPluginPack/actions/workflows/CI_build.yml/badge.svg)](https://github.com/molsonkiko/NppCSharpPluginPack/actions/workflows/CI_build.yml)

[![License](http://img.shields.io/badge/License-Apache_2-red.svg?style=flat)](http://www.apache.org/licenses/LICENSE-2.0)

This is a template for Notepad++ plugins written. The vast majority of the code (certainly all the parts that were really hard to implment) come from [kbilsted's now-archived `NotepadPlusPlusPluginPack.Net`](https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net), with some significant changes, including:

1. `NotepadPlusPlusPluginPack.Net` has an architecture that is (in my opinion) needlessly complicated, because it separates the `PluginInfrastructure` folder containing all the necessary code for connecting with Notepad++ in a completely different directory tree from the rest of the code.
2. This plugin template includes an example of a [non-docking (pop-up) dialog](/docs/README.md#about-form) opened from the plugin menu, a [docking form](/docs/README.md#selections-remembering-form), and a [non-docking non-pop-up form](/docs/README.md#dark-mode-test-form).
3. This plugin template includes a [settings form](/docs/README.md#settings-form) that makes it easy for the maintainer to add, remove or edit settings.
4. This template has a template for [automated tests](/docs/README.md#running-tests) that can be run inside Notepad++, including a template for user interface tests that automatically test forms.

If you have any issues, see if [updating to the latest release](https://github.com/molsonkiko/NppCSharpPluginPack/releases) helps, and then feel free to raise an [issue](https://github.com/molsonkiko/NppCSharpPluginPack/issues) on GitHub. Please be sure to include diagnostic information about your system, Notepad++ version, and plugin version (go to `?->Debug Info...` from the Notepad++ main menu).

[Read the docs.](/docs/README.md)

[Read information about the plugin architecture (some of this may be out of date)](/PluginPackArchitecture.md)

[View past changes.](/CHANGELOG.md)

## Downloads and Installation ##

Go to the [Releases page](https://github.com/molsonkiko/NppCSharpPluginPack/releases) to see past releases.

[Download latest 32-bit version](https://github.com/molsonkiko/NppCSharpPluginPack/raw/main/NppCSharpPluginPack/Release_x86.zip)

You can unzip the 32-bit download to `.\Program Files (x86)\Notepad++\plugins\CSharpPluginPack\CSharpPluginPack.dll`.

[Download latest 64-bit version](https://github.com/molsonkiko/NppCSharpPluginPack/raw/main/NppCSharpPluginPack/Release_x64.zip)

You can unzip the 64-bit download to `C:\Program Files\Notepad++\plugins\CSharpPluginPack\CSharpPluginPack.dll`.

Alternatively, you can follow these [installation instructions](https://npp-user-manual.org/docs/plugins/) to install the latest version of the plugin from Notepad++.

## System Requirements ##

This plugin is verified to work on versions of Notepad++ as old as [v7.3.3](https://notepad-plus-plus.org/downloads/v7.3.3/), but it has some bugs, mainly with HTML/XML tag matching.

It has also been tested and verified to work normally on the newest versions of Notepad++, [v8.6.1](https://notepad-plus-plus.org/downloads/v8.6.1/) and [v8.6.2 RC](https://community.notepad-plus-plus.org/topic/25341/notepad-v8-6-2-release-candidate/19).

Every version of the template works on [Windows 10 May 2019 update](https://blogs.windows.com/windowsexperience/2019/05/21/how-to-get-the-windows-10-may-2019-update/) or later, or you must install [.NET Framework 4.8](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48).

## Acknowledgments ##

* [Kasper B. Graversen](https://github.com/kbilsted) for creating the [plugin pack](https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net) that this is based on.
* [jokedst](https://github.com/jokedst) for making the [CsvQuery plugin](https://github.com/jokedst/CsvQuery) to which I owe the original ideas behind my settings form and my adaptive styling of forms.
* And of course, Don Ho for creating [Notepad++](https://notepad-plus-plus.org/)!
