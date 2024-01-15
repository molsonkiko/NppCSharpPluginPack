# Change Log
All [notable changes](#001---2024-01-13) to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).
 
## [Unreleased] - yyyy-mm-dd
 
### To Be Added

1. toggle highlighting of `close HTML/XML tag` toolbar icon based on whether the setting is true.
2. add more UI tests for [ScintillaGateway.cs](/NppCSharpPluginPack/PluginInfrastructure/ScintillaGateway.cs) and [NotepadPPGateway.cs](/NppCSharpPluginPack/PluginInfrastructure/NotepadPPGateway.cs) methods to make sure they work.

### To Be Changed

- Add better [documentation for plugin menu commands](/docs/README.md#plugin-menu-commands)

### To Be Fixed

- Closing HTML/XML tags works inconsistently in Notepad++ v7.3.3.

## [0.0.2] - (UNRELEASED) YYYY-MM-DD

### Added

1. Ported over (from kbilsted's old template) the [ToolsForMaintainersOfTheProjectTemplate](/ToolsForMaintainersOfTheProjectTemplate/) folder for updating some of the [PluginInfrastructure](/NppCSharpPluginPack/PluginInfrastructure/) files to stay up to date with Notepad++.

### Changed

1. Remove references and links to JsonTools (they now go to this project's GitHub repo).

### Fixed

1. [TestRunner.cs](/NppCSharpPluginPack/Tests/TestRunner.cs) now restores clipboard text after tests.
2. Link label text and background now correctly switches back to defaults when going from a dark theme to default styles.
3. Fix bug where running tests multiple times in a single Notepad++ session causes the user interface tests to fail.

## [0.0.1] - 2024-01-13

### Added

1. [selections remembering form](/docs/README.md#selections-remembering-form)
2. [dark mode test form](/docs/README.md#dark-mode-test-form)
3. [about form](/docs/README.md#about-form)
4. [settings form](/docs/README.md#settings-form)
5. [Basic tests, including user interface tests](/docs/README.md#running-tests)
6. [Plugin menu commands](/docs/README.md#plugin-menu-commands)