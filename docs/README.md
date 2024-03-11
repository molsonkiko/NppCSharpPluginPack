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
```ini
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
- `Escape` moves focus from the form to the editor.
- `Enter` or `Space` while a button is selected clicks that button.

This is all because I registered the controls in the form with the `KeyUp`, `KeyDown`, and `KeyPress` handlers in [NppCSharpPlugin/Forms/NppFormHelper.cs](/NppCSharpPluginPack/Forms/NppFormHelper.cs).

You will notice that, prior to [v0.0.4](/CHANGELOG.md#004---unreleased-yyyy-mm-dd), the DarkModeTestForm discussed below *does not have these nice responses to keys*, because I did not register those handlers for its controls.

## Subclass [FormBase](/NppCSharpPluginPack/Forms/FormBase.cs) (added in [v0.0.3.2](/CHANGELOG.md#004---unreleased-yyyy-mm-dd)) for better forms ##

Beginning with [v0.0.3.2](/CHANGELOG.md#004---unreleased-yyyy-mm-dd), all the forms (except the About form) subclass [FormBase](/NppCSharpPluginPack/Forms/FormBase.cs), which fixes a number of bugs and implements a number of desirable behaviors.

## Registering and unregistering forms with [`NPPM_MODELESSDIALOG`](https://npp-user-manual.org/docs/plugin-communication/#2036-nppm-modelessdialog) ##

Beginning in [v0.0.2](/CHANGELOG.md#002---2024-02-06), all modeless (that is, *not* pop-up) forms are registered in their initialization method with [NppFormHelper.RegisterFormIfModeless](https://github.com/molsonkiko/NppCSharpPluginPack/blob/d3d5aa9e2992d424e07ebdf31fa8b0d53cf26429/NppCSharpPluginPack/Forms/NppFormHelper.cs#L114) and *unregistered* in their [`Dispose` method (in `Designer.cs`)](https://github.com/molsonkiko/NppCSharpPluginPack/blob/d3d5aa9e2992d424e07ebdf31fa8b0d53cf26429/NppCSharpPluginPack/Forms/DarkModeTestForm.Designer.cs#L18) with [NppFormHelper.UnregisterFormIfModeless](https://github.com/molsonkiko/NppCSharpPluginPack/blob/d3d5aa9e2992d424e07ebdf31fa8b0d53cf26429/NppCSharpPluginPack/Forms/NppFormHelper.cs#L128).

This is not strictly necessary, but for versions of Notepad++ 8.6.1 and later, the `Ctrl+C` and `Ctrl+X` keyboard shortcuts could be broken for all modeless forms that are not registered. Note that in Notepad++ 8.6.2 (and probably later as well), Ctrl+C/X are only broken if you uncheck `Settings->Preferences...->Editing->Enable Copy/Cut Line without selection` from the Notepad++ main menu and then do not select any text in the current document.

Unfortunately, registering forms in this way has an unusual effect on how the `Tab` key moves through controls. Normally the tab order of controls is set by their `TabIndex` attribute, but __if a form has been registered by `NppFormHelper.RegisterFormIfModeless`, its controls' tab order is the order in which they were added to their parent in the `Designer.cs` file.__

For example, consider a file with the following three controls, with (`TabIndex` value, place in `this.Controls.Add` order) in parentheses:
- `FooTextBox` (`TabIndex`=0, `this.Controls.Add` place = 2)
- `BarComboBox` (`TabIndex`=1, `this.Controls.Add` place = 0)
- `BazButton` (`TabIndex`=2, `this.Controls.Add` place = 1)

If the form __*has not* been registered__ with `NppFormHelper.RegisterFormIfModeless` ([see `PopupDialog.Designer.cs` for example](https://github.com/molsonkiko/NppCSharpPluginPack/blob/d3d5aa9e2992d424e07ebdf31fa8b0d53cf26429/NppCSharpPluginPack/Forms/PopupDialog.Designer.cs#L46)), the tab order follows `TabIndex` values (but *only after adding a KeyUp listener that calls [NppFormHelper.GenericKeyUpHandler](https://github.com/molsonkiko/NppCSharpPluginPack/blob/7e8ae6c5a0c31b3266938e1a61c5c8e505a77ce6/NppCSharpPluginPack/Forms/NppFormHelper.cs#L46)*), that is:
1. `FooTextBox`
2. `BarComboBox`
3. `BazButton`

If the form __*has* been registered__ with `NppFormHelper.RegisterFormIfModeless` ([see `DarkModeTestForm.Designer.cs` for example](https://github.com/molsonkiko/NppCSharpPluginPack/blob/d3d5aa9e2992d424e07ebdf31fa8b0d53cf26429/NppCSharpPluginPack/Forms/DarkModeTestForm.Designer.cs#L246)), the tab order follows `this.Controls.Add` order, that is:
1. `BarComboBox`
2. `BazButton`
3. `FooTextBox`

In addition, registering forms in this way causes `Enter` to no longer add newlines in textboxes. This behavior is compensated for by the [`PressEnterInTextBoxHandler`](https://github.com/molsonkiko/NppCSharpPluginPack/blob/574e1964e1e3fd68c0a7a499504e89cf7d64e493/NppCSharpPluginPack/Forms/NppFormHelper.cs#L105).

Finally, some keypresses no longer trigger `KeyDown` and `KeyPress` events if a form is registered in this way.

## Dark mode test form ##

This form exists solely for the purposes of testing how this plugin pack's automatic styling works.

You can see the effect of changing the Notepad++ stylers in this image of all the forms with default stylers:

![All forms with default styles](/docs/all%20forms%20default%20style.PNG)

In dark mode:

![All forms in dark mode](/docs/all%20forms%20dark%20mode.PNG)

In light mode, with the MossyLawn [theme](https://npp-user-manual.org/docs/themes/):

![All files in MossyLawn theme](/docs/all%20forms%20MossyLawn.PNG)

## Popup dialog ##

The popup dialog (*added in [v0.0.2](/CHANGELOG.md#002---2024-02-06)*) exists solely to illustrate how to properly configure pop-up dialogs (as opposed to modeless dialogs like the DarkModeTestForm and the SelectionRememberingForm).

Here are some important ways that the popup dialog differs from other dialogs:
- `true` should be passed as the `isModal` argument for every method that takes an `isModal` argument (`NppFormHelper.RegisterFormIfModeless`, `NppFormHelper.GenericKeyUpHelper`, and `NppFormHelper.UnregisterFormIfModeless`), because the pop-up dialog blocks Notepad++ until it is closed.
- The `ProcessDialogKey` method must be overridden as shown in `PopupDialog.cs`.
- Each control's place in the tab order is controlled by its `TabIndex` attribute. This is because this is a modal dialog.

![Popup dialog](/docs/popup%20dialog.PNG)

## Loading third-party dependencies ##

There are two ways that I (Mark J. Olson) know of to incorporate third-party dependencies into the project, using [NuGet](https://www.nuget.org/) and loading locally installed DLL's. Each will be covered below.

Regardless of which type of dependency you use, __make sure that the depenendencies work for both 32-bit and 64-bit Notepad++.__

Note that the first version of this project to directly support 3rd-party DLL's in this way is `0.0.3.1`.

### Including NuGet packages in your project ###

I have tested this (as of version `0.0.3.1`) by installing [ExcelDataReader 3.6.0](https://www.nuget.org/packages/ExcelDataReader/3.6.0), adding some ExcelDataReader method calls to the [PopupDialog](#popup-dialog), and verifying that the method calls run successfully.

1. Install the NuGet package. In Visual Studio, this entails going to `Project->Manage NuGet packages...` from the main menu, then installing a package.
2. Build the project as normal. The NuGet package should be usable as expected.

### Including locally installed DLL's in your project ###

This is demonstrated with the [ExampleDependency](/ExampleDependency) example dependency, which is referenced in the [Popup Dialog](/NppCSharpPluginPack/Forms/PopupDialog.cs).

1. Add a *64-bit* build of the DLL to the [NppCSharpPluginPack\Dependencies\x64](/NppCSharpPluginPack/Dependencies/x64) directory.
2. Add a *32-bit* build of the DLL to the [NppCSharpPluginPackDependencies\x86](/NppCSharpPluginPack/Dependencies/x86) directory.
3. Build the project for 64-bit and 32-bit Notepad++. Verify that any 3rd-party DLL's are usable as normal.

## Running tests ##

I (Mark J. Olson) believe that without a robust automated test suite, it is hard to make major changes to any large project without breaking things unexpectedly. Over the course of developing my JsonTools plugin, I developed a strategy for running automated tests and performance benchmarks inside of Notepad++.

My test framework works as follows:
1. [TestRunner.cs](/NppCSharpPluginPack/Tests/TestRunner.cs) will start running tests, and open a new file where the test report will go. You can control whether a test will be skipped in some cases, say if the Notepad++ version is too old.
2. You create tests of code in a test file, like [SliceTests.cs](/NppCSharpPluginPack/Tests/SliceTests.cs).
    - This test file will need keep track of how many tests pass and fail. When a test fails, it will write a message to the test report file.
    - Test files like this are a good place to test low-level code that does not depend on interactions with Notepad++
3. You can also run [user interface tests](/NppCSharpPluginPack/Tests/UserInterfaceTests.cs) that tests the way the plugin actually interacts with Notepad++ (for instance, how plugin commands edit files)
4. Once the tests are done running, you will see a report that looks like [most recent errors.txt](/most%20recent%20errors.txt)

## Assembly info and the changelog ##

I __strongly recommend__ updating the [NppCSharpPluginPack/Properties/AssemblyInfo.cs](https://github.com/molsonkiko/NppCSharpPluginPack/blob/main/NppCSharpPluginPack/Properties/AssemblyInfo.cs) file every time you make a commit to your plugin's repository. If you look at the [commit history of this repository](https://github.com/molsonkiko/NppCSharpPluginPack/commits/main/) you will see that every commit that changes a `.cs` file has a different assembly version. This makes life easier for you and your users.

I also recommend that your changes to `AssemblyInfo.cs` comply with [the semantic versioning RFC](https://semver.org/), and that you update the [changelog](/CHANGELOG.md) to track your changes.