NppCSharpPluginPack Overview
====================

Curious about the architecture of the plugin (like what [`ScintillaGateway.cs`](/NppCSharpPluginPack/PluginInfrastructure/ScintillaGateway.cs) and [`NotepadPPGateway.cs`](/NppCSharpPluginPack/PluginInfrastructure/NotepadPPGateway.cs) are doing)? [Read the plugin architecture documentation](/PluginPackArchitecture.md)!

For general information on C#, I highly recommend using the [official docs at Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/).

## Changing the name of the plugin ##

Go to [NppCSharpPluginPack/Main.cs](/NppCSharpPluginPack/Main.cs) and change the `PluginName` field.

You will also want to go to [NppCSharpPluginPack/Properties/AssemblyInfo.cs](/NppCSharpPluginPack/Properties/AssemblyInfo.cs) and change all the information in there.

## Where the config data goes ##

This plugin puts all config data in the `plugins/config/CSharpPluginPack` subdirectory of wherever Notepad++ puts its config data (this is usually `%AppData%/Roaming/Notepad++`).

Changing `Main.PluginName` will automatically change the name of the config subdirectory.

## Plugin menu commands ##

In [the `static internal void CommandMenuInit()` method of `Main.cs`](/NppCSharpPluginPack/Main.cs), you can add references to plugin commands.

You can also add toolbar icons, which you can customize by editing the PNG files in the [NppCSharpPluginPack/Resources](/NppCSharpPluginPack/Resources/) folder and then running [toolbar icons.bat](/NppCSharpPluginPack/Resources/toolbar%20icons.bat), which recreates all the bitmaps and icons from the PNG files.

Take a look at the plugin commands in `Main.cs`. If you're new to C#, they will help you learn:
- How to run a task on a separate thread
- How to work with structs and unsafe methods
- How to use basic data structures like lists and tuples
- How to use regular expressions.

And of course, they will help you familiarize yourself with how this plugin pack interfaces with Notepad++ and the Scintilla editor component.

## Settings form ##

The settings form looks something like this:

![Settings form](/docs/settings%20form.PNG)

You can customize the settings in [NppCSharpPluginPack/Utils/Settings.cs](/NppCSharpPluginPack/Utils/Settings.cs), where they are arranged like so:
```cs
#region STYLING
[Description("Use the same colors as the editor window for this plugin's forms?"),
    Category("Styling"), DefaultValue(true)]
public bool use_npp_styling { get; set; }
#endregion
#region TESTING
[Description("Ask before running tests, because the test can hijack the user's clipboard"),
    Category("Testing"), DefaultValue(AskUserWhetherToDoThing.ASK_BEFORE_DOING)]
public AskUserWhetherToDoThing ask_before_testing { get; set; }
#endregion
```
When you change the settings, they will be written to `CSharpPluginPack.ini` (tied to`Main.PluginName`).

The `CSharpPluginPack.ini` file looks something like this:
```cs
[Styling]
; Use the same colors as the editor window for this plugin's forms?
use_npp_styling=True

[Testing]
; Ask before running tests, because the test can hijack the user's clipboard
ask_before_testing=DO_WITHOUT_ASKING
```

## About form ##

This form is a pop-up dialog with a link to the GitHub repository of your plugin (set to `Main.PluginRepository`). It looks like this.

![About form](/docs/about%20form.PNG)

See [NppCSharpPluginPack/Forms/AboutForm.cs](/NppCSharpPluginPack/Forms/AboutForm.cs) for how to change this. You will need to use the Windows Form Designer GUI in Visual Studio.

## Selections remembering form ##

This remembers the user's selections, and looks like this:

![Selections remembering form](/docs/selections%20remembering%20form.PNG)

You can see the relevant code in [NppCSharpPluginPack/Forms/SelectionRememberingForm.cs](/NppCSharpPluginPack/Forms/SelectionRememberingForm.cs). Understanding this code will help you understand file IO in C#, the way the editor component remembers selections, and Windows Forms.

One thing to note is that this form responds to keys in intuitive and useful ways:
- `Tab` navigates forward through controls, `Shift-Tab` navigates backward
- `Escape` moves focus from the form to the editor.
- `Enter` or `Space` while a button is selected clicks that button.

This is all because I registered the controls in the form with the `KeyUp`, `KeyDown`, and `KeyPress` handlers in [NppCSharpPlugin/Forms/NppFormHelper.cs](/NppCSharpPluginPack/Forms/NppFormHelper.cs).

You will notice that the DarkModeTestForm discussed below does not have these nice responses to keys, because I did not register those handlers for its controls.

## Dark mode test form ##

This form exists solely for the purposes of testing how this plugin pack's automatic styling works.

You can see the effect of changing the Notepad++ stylers in this image of all the forms with default stylers:

![All forms with default styles](/docs/all%20forms%20default%20style.PNG)

In dark mode:

![All forms in dark mode](/docs/all%20forms%20dark%20mode.PNG)

In light mode, with the MossyLawn [theme](https://npp-user-manual.org/docs/themes/):

![All files in MossyLawn theme](/docs/all%20forms%20MossyLawn.PNG)

## Running tests ##

I (Mark J. Olson) believe that without a robust automated test suite, it is hard to make major changes to any large project without breaking things unexpectedly. Over the course of developing my JsonTools plugin, I developed a strategy for running automated tests and performance benchmarks inside of Notepad++.

My test framework works as follows:
1. [TestRunner.cs](/NppCSharpPluginPack/Tests/TestRunner.cs) will start running tests, and open a new file where the test report will go. You can control whether a test will be skipped in some cases, say if the Notepad++ version is too old.
2. You create tests of code in a test file, like [SliceTests.cs](/NppCSharpPluginPack/Tests/SliceTests.cs).
    - This test file will need keep track of how many tests pass and fail. When a test fails, it will write a message to the test report file.
    - Test files like this are a good place to test low-level code that does not depend on interactions with Notepad++
3. You can also run [user interface tests](/NppCSharpPluginPack/Tests/UserInterfaceTests.cs) that tests the way the plugin actually interacts with Notepad++ (for instance, how plugin commands edit files)
4. Once the tests are done running, you will see a report that looks like [most recent errors.txt](/most%20recent%20errors.txt)