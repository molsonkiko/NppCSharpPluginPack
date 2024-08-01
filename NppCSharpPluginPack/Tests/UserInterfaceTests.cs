using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NppDemo.Utils;
using Kbg.NppPluginNET;

namespace NppDemo.Tests
{
    public enum FileManipulation
    {
        /// <summary>
        /// OpenFile(index: int)<br></br>
        /// opens the index^th UI test file (the first one opened has index 0)<br></br>
        /// if index &gt;= than the number of files already opened,
        /// open more UI test files until this is no longer true
        /// </summary>
        OpenFile,
        /// <summary>
        /// Overwrite(newText: string)<br></br>
        /// overwrite the current file with newText
        /// </summary>
        Overwrite,
        /// <summary>
        /// Select(params string[] startEnds)<br></br>
        /// each argument must be a string of the form "selection start,selection end"<br></br>
        /// Make selections starting and ending at the specified places
        /// </summary>
        Select,
        /// <summary>
        /// SelectWholeDoc()<br></br>
        /// Select all the text in the document
        /// </summary>
        SelectWholeDoc,
        /// <summary>
        /// InsertText(int start, string text)<br></br>
        /// Insert text at position start in the document (0-indexed)
        /// </summary>
        InsertText,
        /// <summary>
        /// DeleteText(int start, int length)<br></br>
        /// Delete length characters from the current document starting at position start.
        /// </summary>
        DeleteText,
        /// <summary>
        /// CompareSelections(params string[] startEnds) -&gt; bool<br></br>
        /// each argument must be a string of the form "selection start,selection end"<br></br>
        /// Returns true if and only if the current selections in the current document
        /// DO NOT start and end at the specified places
        /// </summary>
        CompareSelections,
        /// <summary>
        /// CompareText(string expectedText) -&gt; bool<br></br>
        /// Returns true and only if the current text in the document is NOT equal to expectedText
        /// </summary>
        CompareText,
        /// <summary>
        /// CompareClipboard(string expectedText) -&gt; bool<br></br>
        /// Returns true if and only if the current text in the clipboard is NOT equal to expectedText
        /// </summary>
        CompareClipboard,
        /// <summary>
        /// OpenSelectionRememberingForm()<br></br>
        /// Open the selection remembering form
        /// </summary>
        OpenSelectionRememberingForm,
        /// <summary>
        /// SelectWithSelectionRememberingForm(string selectionText) -&gt; bool<br></br>
        /// - if selectionText is null, leave the selection remembering form's text box unchanged.<br></br>
        /// - Otherwise, enter selectionText into the selection remembering form's text box.<br></br>
        /// Then, click the "Select all regions in the text box above" button.<br></br>
        /// Return true if and only if the SelectionRememberingForm is NOT open.
        /// </summary>
        SelectWithSelectionRememberingForm,
        /// <summary>
        /// SaveSelectionsWithSelectionRememberingForm() -&gt; bool<br></br>
        /// click the "Save current selections to file" button in the selection remembering form<br></br>
        /// Return true if and only if the SelectionRememberingForm is NOT open.
        /// </summary>
        SaveSelectionsWithSelectionRememberingForm,
        /// <summary>
        /// SaveSelectionsWithSelectionRememberingForm() -&gt; bool<br></br>
        /// Click the "Load selections from config file" button in the selection remembering form<br></br>
        /// Then, click the "Select all regions in the text box above" button.<br></br>
        /// Return true if and only if the SelectionRememberingForm is NOT open.
        /// </summary>
        LoadSelectionsWithSelectionRememberingForm,
        /// <summary>
        /// SaveSelectionsWithSelectionRememberingForm() -&gt; bool<br></br>
        /// click the "Copy current selections to clipboard" button in the selection remembering form<br></br>
        /// Return true if and only if the SelectionRememberingForm is NOT open.
        /// </summary>
        CopySelectionsWithSelectionRememberingForm,
    }

    public class UserInterfaceTester
    {
        private static List<string> filenamesUsed;

        public static string lastClipboardValue = null;


        /// <summary>
        /// Run a command (see below switch statement for options) to manipulate the test file.
        /// Returns true if one of the "compare" commands (compare_text, compare_selections, compare_treeview) fails the test
        /// </summary>
        /// <param name="command">see the FileManipulation enum for a description of each command</param>
        /// <param name="messages">a list of messages already sent by past file manipulations (add a message on failure or success)</param>
        /// <param name="args">arguments for the FileManipulation</param>
        /// <returns></returns>
        public static bool ExecuteFileManipulation(FileManipulation command, List<string> messages, params object[] args)
        {
            string activeFname = Main.activeFname;
            string correctText, gotText;
            switch (command)
            {
            case FileManipulation.OpenFile:
                int fileIdx = (int)args[0];
                string ext = (args.Length > 1 && args[1] is string extension) ? extension : "json";
                string filename = OpenUITestFile(fileIdx);
                messages.Add($"Opened file {filename}");
                break;
            case FileManipulation.Overwrite:
                var text = (string)args[0];
                messages.Add($"overwrite file with\r\n{text}");
                Npp.editor.SetText(text);
                break;
            case FileManipulation.Select:
                var startEndStrings = args.Select(x => (string)x).ToArray();
                messages.Add($"select {SelectionManager.StartEndListToString(startEndStrings)}");
                SelectionManager.SetSelectionsFromStartEnds(startEndStrings);
                break;
            case FileManipulation.SelectWholeDoc:
                if (!Npp.TryGetLengthAsInt(out int len, false))
                {
                    messages.Add("FAIL: buffer was too long");
                    return true;
                }
                var wholeSelStr = $"0,{len}";
                messages.Add("select whole document");
                SelectionManager.SetSelectionsFromStartEnds(new string[] { wholeSelStr });
                break;
            case FileManipulation.InsertText:
                var start = (int)args[0];
                text = (string)args[1];
                messages.Add($"insert {Npp.StrToString(text, false)} at {start}");
                Npp.editor.InsertText(start, text);
                break;
            case FileManipulation.DeleteText:
                start = (int)args[0];
                int length = (int)args[1];
                messages.Add($"delete {length} chars starting at {start}");
                Npp.editor.DeleteRange(start, length);
                break;
            case FileManipulation.CompareSelections:
                Npp.editor.GrabFocus();
                var gotSelections = SelectionManager.GetSelectedRanges();
                string correctSelStr = SelectionManager.StartEndListToString(args.Select(x => (string)x));
                string gotSelStr = SelectionManager.StartEndListToString(gotSelections);
                if (correctSelStr != gotSelStr)
                {
                    messages.Add($"FAIL: expected selections\r\n{correctSelStr}\r\nGOT\r\n{gotSelStr}");
                    return true;
                }
                messages.Add("compare_selections passed");
                break;
            case FileManipulation.CompareText:
                correctText = (string)args[0];
                if (!Npp.TryGetText(out gotText, false))
                {
                    messages.Add("FAIL: buffer was too long");
                    return true;
                }
                if (correctText != gotText)
                {
                    messages.Add($"FAIL: expected text\r\n{correctText}\r\nGOT\r\n{gotText}");
                    return true;
                }
                messages.Add("compare_text passed");
                break;
            case FileManipulation.CompareClipboard:
                correctText = (string)args[0];
                gotText = Clipboard.GetText();
                if (correctText != gotText)
                {
                    messages.Add($"FAIL: expected clipboard to contain text\r\n{correctText}\r\nGOT\r\n{gotText}");
                    return true;
                }
                messages.Add("compare_clipboard passed");
                break;
            case FileManipulation.OpenSelectionRememberingForm:
                if (Main.selectionRememberingForm == null || Main.selectionRememberingForm.IsDisposed || !Main.selectionRememberingForm.Visible)
                    Main.OpenSelectionRememberingForm();
                messages.Add("open selection remembering form");
                break;
            case FileManipulation.SelectWithSelectionRememberingForm:
                if (Main.selectionRememberingForm == null || Main.selectionRememberingForm.IsDisposed)
                {
                    messages.Add($"FAIL: selection remembering form not open when running {command}");
                    return true;
                }
                if (args[0] is string selections)
                    Main.selectionRememberingForm.SelectionStartEndsBox.Text = selections;
                Main.selectionRememberingForm.SetSelectionsFromStartEndsButton.PerformClick();
                messages.Add($"Set selections to \"{Main.selectionRememberingForm.SelectionStartEndsBox.Text}\" with the selection remembering form");
                break;
            case FileManipulation.CopySelectionsWithSelectionRememberingForm:
                if (Main.selectionRememberingForm == null || Main.selectionRememberingForm.IsDisposed)
                {
                    messages.Add($"FAIL: selection remembering form not open when running {command}");
                    return true;
                }
                Main.selectionRememberingForm.CopySelectionsToStartEndsButton.PerformClick();
                // need to do this, otherwise your clipboard will be overwritten
                lastClipboardValue = Clipboard.GetText();
                messages.Add("Copied selections to clipboard with selection remembering form");
                break;
            case FileManipulation.SaveSelectionsWithSelectionRememberingForm:
                if (Main.selectionRememberingForm == null || Main.selectionRememberingForm.IsDisposed)
                {
                    messages.Add($"FAIL: selection remembering form not open when running {command}");
                    return true;
                }
                Main.selectionRememberingForm.SaveCurrentSelectionsToFileButton.PerformClick();
                messages.Add("Saved selections to config file with selection remembering form");
                break;
            case FileManipulation.LoadSelectionsWithSelectionRememberingForm:
                if (Main.selectionRememberingForm == null || Main.selectionRememberingForm.IsDisposed)
                {
                    messages.Add($"FAIL: selection remembering form not open when running {command}");
                    return true;
                }
                Main.selectionRememberingForm.LoadSelectionsFromFileButton.PerformClick();
                Main.selectionRememberingForm.SetSelectionsFromStartEndsButton.PerformClick();
                messages.Add("Loaded selections from config file with selection remembering form");
                break;
            default:
                throw new ArgumentException($"Unrecognized command {command}");
            }
            return false;
        }

        public static bool Test()
        {
            var testcases = new List<(FileManipulation command, object[] args)>
            {
                (FileManipulation.Overwrite, new object[]{"[1, 2, false]\r\n{\"a\": 3, \"b\": 1.5}\r\n[{\"c\": [null]}, -7]"}),
                // make sure the CompareText doesn't give false positives
                (FileManipulation.CompareText, new object[]{"[1, 2, false]\r\n{\"a\": 3, \"b\": 1.5}\r\n[{\"c\": [null]}, -7]"}),
                (FileManipulation.Select, new object[]{ "0,13", "15,33", "35,54" }),
                // make sure the CompareSelection doesn't give false positives
                (FileManipulation.CompareSelections, new object[]{ "0,13", "15,33", "35,54" }),
                (FileManipulation.InsertText, new object[]{3, " \"foo\","}),
                (FileManipulation.CompareText, new object[]{"[1, \"foo\", 2, false]\r\n{\"a\": 3, \"b\": 1.5}\r\n[{\"c\": [null]}, -7]"}),
                (FileManipulation.CompareSelections, new object[]{ "42,42" }),
                (FileManipulation.Select, new object[]{ "5,5" }),
                (FileManipulation.CompareSelections, new object[]{ "5,5" }),
                (FileManipulation.DeleteText, new object[]{10, 4}),
                (FileManipulation.CompareText, new object[]{"[1, \"foo\",false]\r\n{\"a\": 3, \"b\": 1.5}\r\n[{\"c\": [null]}, -7]"}),
                (FileManipulation.DeleteText, new object[]{16, 1}),
                (FileManipulation.InsertText, new object[]{17, "bar\tbaz\n"}),
                (FileManipulation.CompareText, new object[]{"[1, \"foo\",false]\nbar\tbaz\n{\"a\": 3, \"b\": 1.5}\r\n[{\"c\": [null]}, -7]"}),
                // test selection remembering form
                (FileManipulation.OpenSelectionRememberingForm, new object[]{}),
                (FileManipulation.SelectWithSelectionRememberingForm, new object[]{"1,2 15,20\r\n4,7\t9,9"}),
                (FileManipulation.CompareSelections, new object[]{ "1,2", "4,7", "9,9", "15,20" }),
                // save the current selections (will load them later)
                (FileManipulation.SaveSelectionsWithSelectionRememberingForm, new object[]{}),
                (FileManipulation.Select, new object[]{"3,3"}),
                (FileManipulation.CopySelectionsWithSelectionRememberingForm, new object[]{}),
                (FileManipulation.CompareClipboard, new object[]{"3,3"}),
                (FileManipulation.Select, new object[]{"19,24", "10,8"}),
                (FileManipulation.CopySelectionsWithSelectionRememberingForm, new object[]{}),
                (FileManipulation.CompareClipboard, new object[]{"8,10 19,24"}),
                // load the selections we saved earlier (make sure they override current selections)
                (FileManipulation.LoadSelectionsWithSelectionRememberingForm, new object[]{}),
                (FileManipulation.CompareSelections, new object[]{ "1,2", "4,7", "9,9", "15,20" }),
                (FileManipulation.SelectWithSelectionRememberingForm, new object[]{null}),
                (FileManipulation.CompareSelections, new object[]{ "1,2", "4,7", "9,9", "15,20" }),
                // test out-of-order selections
                (FileManipulation.SelectWithSelectionRememberingForm, new object[]{"34,30"}),
                (FileManipulation.CompareSelections, new object[]{ "30,34" }),
                (FileManipulation.Select, new object[]{ "41,41" }),
                // try saving selections again
                (FileManipulation.SaveSelectionsWithSelectionRememberingForm, new object[]{}),
                (FileManipulation.SelectWithSelectionRememberingForm, new object[]{"15,14 9,3"}),
                (FileManipulation.CompareSelections, new object[]{"3,9", "14,15"}),
                // try loading selections again
                (FileManipulation.LoadSelectionsWithSelectionRememberingForm, new object[]{}),
                (FileManipulation.CompareSelections, new object[]{ "41,41" }),

            };

            var messages = new List<string>();
            int failures = 0;
            string previouslyOpenFname = Npp.notepad.GetCurrentFilePath();
            filenamesUsed = new List<string>();
            string UITestFileName = OpenUITestFile(0);
            // IMPORTANT: remember all the user's settings before the tests
            
            // remember what the user's clipboard was before tests start, because the tests hijack the clipboard and that's not nice
            string clipboardValueBeforeTests = Clipboard.GetText();
            // require these settings for the UI tests alone
            
            // run all the commands
            int lastFailureIndex = messages.Count;
            foreach ((FileManipulation command, object[] args) in testcases)
            {
                try
                {
                    int messageCountBefore = messages.Count;
                    bool failed = ExecuteFileManipulation(command, messages, args);
                    if (failed)
                    {
                        failures++;
                        lastFailureIndex = messages.Count;
                    }
                    if (messages.Count == messageCountBefore)
                    {
                        failures++;
                        lastFailureIndex = messages.Count;
                        messages.Add($"File manipulation {command} did not issue a message when failed={failed}");
                    }
                }
                catch (Exception ex)
                {
                    failures++;
                    messages.Add("While running command " + command + " with args [" + string.Join(", ", args) + "], got exception\r\n" + ex);
                    lastFailureIndex = messages.Count;
                }
            }
            // go back to the test file and show the results
            Npp.notepad.OpenFile(previouslyOpenFname);
            if (failures > 0)
            {
                // show all the messages up to the last failure
                Npp.AddLine(string.Join("\r\n", messages.LazySlice(0, lastFailureIndex)));
            }
            Npp.AddLine($"Failed {failures} tests");
            Npp.AddLine($"Passed {testcases.Count - failures} tests");
            // IMPORTANT: restore old settings if you changed them for the tests
            
            // if the user's clipboard is still set to whatever we most recently hijacked it with, reset it to whatever it was before the tests
            // this won't work if their clipboard contained non-text data beforehand, but it's better than nothing
            if (Clipboard.GetText() == lastClipboardValue && !(clipboardValueBeforeTests is null) && clipboardValueBeforeTests.Length > 0)
            {
                Npp.TryCopyToClipboard(clipboardValueBeforeTests);
            }
            return failures > 0;
        }

        private static string OpenUITestFile(int fileIdx)
        {
            while (fileIdx >= filenamesUsed.Count)
            {
                Npp.notepad.FileNew();
                string newFilename = Npp.notepad.GetCurrentFilePath();
                filenamesUsed.Add(newFilename);
            }
            string filename = filenamesUsed[fileIdx];
            Npp.notepad.OpenFile(filename);
            return filename;
        }
    }
}
