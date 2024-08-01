# Change Log
All [notable changes](#003---2024-02-26) to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).
 
## [Unreleased] - yyyy-mm-dd
 
### To Be Added

1. toggle highlighting of `close HTML/XML tag` toolbar icon based on whether the setting is true.
2. add more UI tests for [ScintillaGateway.cs](/NppCSharpPluginPack/PluginInfrastructure/ScintillaGateway.cs) and [NotepadPPGateway.cs](/NppCSharpPluginPack/PluginInfrastructure/NotepadPPGateway.cs) methods to make sure they work.
3. In [ToolsForMaintainersOfTheProjectTemplate](/ToolsForMaintainersOfTheProjectTemplate/), I need to add auto-generation of calls to `GetNullStrippedStringFromMessageThatReturnsLength(SciMsg msg, IntPtr wParam=default)` instead of the old fixed-size 10000-byte buffers

### To Be Changed

- Add better [documentation for plugin menu commands](/docs/README.md#plugin-menu-commands)

### To Be Fixed

- Closing HTML/XML tags works inconsistently in Notepad++ v7.3.3.
- Holding down `Enter` in a multiline textbox does not add multiple new lines; it only adds one newline on keyup.

## [0.0.4] - (UNRELEASED) YYYY-MM-DD

### Added

1. Make it much easier to [include third-party dependencies](/docs/README.md#loading-third-party-dependencies) in your plugin.
2. Added the ability to [translate the plugin into other languages](/README.md#translating-your-plugin-to-another-language).
3. Made it so all forms subclass a base class, making it easier to implement recommended methods.
4. `Npp.TryGetLengthAsInt` and `Npp.TryGetText` methods, which gracefully handle files that are too large to put all their text in a string.

### Fixed

1. Fixed issue where clicking buttons on floating docking dialogs could sometimes cause Notepad++ to hang forever (see [CsvLint issue 83](https://github.com/BdR76/CSVLint/issues/83) for a detailed explanation).
2. Fix `SCNotification` byte alignment issue in 64-bit Notepad++ by making `annotationLinesAdded` field an `IntPtr`, which has been the correct type for that field since [between Notepad++ 7.6.6 and 7.7](https://github.com/notepad-plus-plus/notepad-plus-plus/blob/37c4b894cc247d1ee6976bc1a1b66cfed4b7774e/scintilla/include/Scintilla.h#L1227). Note that *this is a potentially breaking change for 64-bit Notepad++ 7.6.6 or older*, but there's a ton of other bit rot for such old Notepad++ anyway.
3. Fix error due to assuming that "." (the current directory according to the filesystem) will always point to the path to the Notepad++ executable; this is *almost always true*, but can be broken due to at least one known weird interaction (the one molsonkiko is familiar with concerns the `New script` functionality of the PythonScript plugin).
4. Fix bug where, if a setting in the config file had an invalid value (for example, a numeric setting having a value of `blah`), there might be an uncaught exception that would cause Notepad++ to crash. This bug appears to be most likely to occur when the localization is *not* set to `en-us`.
5. Fix bug (related to fixed bug 4 above) where *all settings with non-integer floating-point values* would cause Notepad++ to crash on start-up if the localization used `,` as the decimal separator. 
6. Fix bug with [makerelease.bat](/makerelease.bat) where it did not properly copy the dependency DLLs into the zip files.
7. Fix bug when too-large integers are entered in the selection-remembering form.

## [0.0.3] - 2024-02-26

### Added

1. Added new `Allocate indicators demo`, demonstrating how to allocate and use indicators to style text.
2. Demo on the `SCN_MODIFIED` and `NPPN_GLOBALMODIFIED` notifications, which are used to track when a document is modified.

### Changed

1. Update method of building a DLL to [remove a dependency on .NET Framework 3.5](https://github.com/molsonkiko/NppCSharpPluginPack/pull/4).
    - This also allows the user the specify whatever Notepad++ directory they wish, using the `NppPath32` (to specify 32-bit Notepad++ directory) and `NppPath64` (to specify 64-bit directory) variables from the command line like so: `msbuild /p:NppPath32="D:\portable\npp\x86";NppPath64="somewhere\else\x64" ...`

### Fixed

1. Greatly reduced unnecessary calls to `NPPM_GETCURRENTLANGTYPE` in `DoInsertHtmlCloseTag` by caching the lexer language whenever the lexer language changes or a buffer is opened.
2. Bug where hitting `Enter` in a multiline textbox would not add a new line.

## [0.0.2] - 2024-02-06

### Added

1. Ported over (from kbilsted's old template) the [ToolsForMaintainersOfTheProjectTemplate](/ToolsForMaintainersOfTheProjectTemplate/) folder for updating some of the [PluginInfrastructure](/NppCSharpPluginPack/PluginInfrastructure/) files to stay up to date with Notepad++.
2. Added new [PopupDialog form](/docs/README.md#popup-dialog), which demonstrates how to configure a pop-up dialog that has select-able fields like textboxes or buttons.

### Changed

1. Remove references and links to JsonTools (they now go to this project's GitHub repo).

### Fixed

1. [TestRunner.cs](/NppCSharpPluginPack/Tests/TestRunner.cs) now restores clipboard text after tests.
2. Link label text and background now correctly switches back to defaults when going from a dark theme to default styles.
3. Fix bug where running tests multiple times in a single Notepad++ session causes the [user interface tests](/docs/README.md#running-tests) to fail.
4. [Fix bug introduced in Notepad++ 8.6.1 where `Ctrl+C` and `Ctrl+X` would not work in the text fields of forms.](/docs/README.md#registering-and-unregistering-forms-with-nppm_modelessdialog)

## [0.0.1] - 2024-01-13

### Added

1. [selections remembering form](/docs/README.md#selections-remembering-form)
2. [dark mode test form](/docs/README.md#dark-mode-test-form)
3. [about form](/docs/README.md#about-form)
4. [settings form](/docs/README.md#settings-form)
5. [Basic tests, including user interface tests](/docs/README.md#running-tests)
6. [Plugin menu commands](/docs/README.md#plugin-menu-commands)