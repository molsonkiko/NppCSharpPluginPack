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
- Avoid plugin crash when too-large int values are entered in the selection-remembering form.
- Holding down `Enter` in a multiline textbox does not add multiple new lines; it only adds one newline on keyup.

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