/*
A test runner for all of this package.
*/
using System;
using System.Collections.Generic;
using NppDemo.Utils;
using Kbg.NppPluginNET;
using System.IO;

namespace NppDemo.Tests
{
    public class TestRunner
    {
        public static void RunAll()
        {
            if (!Npp.AskBeforeDoingSomething(Main.settings.ask_before_testing,
                "Running tests can potentially overwrite the contents of your clipboard. Do you still want to run tests?",
                    "Do you want to run tests?"))
                return;

            Npp.notepad.FileNew();
            string header = $"Test results for {Main.PluginName} v{Npp.AssemblyVersionString()} on Notepad++ {Npp.nppVersionStr}\r\nNOTE: Ctrl-F (regular expressions *on*) for \"Failed [1-9]\\d*\" to find all failed tests";
            Npp.AddLine(header);

            string big_random_fname = Path.Combine(Npp.pluginDllDirectory, "testfiles", "big_silly_example.tsv");
            var tests = new (Func<bool> tester, string name, bool onlyIfNpp8Plus)[]
            {
                (SliceTester.Test, "slice extension", false),
                
                (UserInterfaceTester.Test, "UI tests", true),
                
                //because Visual Studio runs a whole bunch of other things in the background
                //     when I build my project, the benchmarking suite
                //     makes my code seem way slower than it actually is when it's running unhindered.
                //     * *To see how fast the code actually is, you need to run the executable outside of Visual Studio.**
                (() => Benchmarker.BenchmarkSomethingThatReadsAFile(
                    new (string, string)[] {
                        ("foo", "test1"
                        ),
                        ("bar", "test2"
                        ),
                    },
                    big_random_fname,
                    "My benchmarks",
                    32), 
                    "Performance of something",
                    true
                ),
            };

            var failures = new List<string>();
            var skipped = new List<string>();
            bool hasExplainedSkipLessThanNppV8 = false;

            foreach ((Func<bool> tester, string name, bool onlyIfNpp8Plus) in tests)
            {
                if (onlyIfNpp8Plus && !Npp.nppVersionAtLeast8)
                {
                    // Notepad++ versions less than 8 (or something around 8)
                    // don't have separate plugin folders for each plugin, so the tests that involve reading files
                    // will cause the plugin to crash
                    if (!hasExplainedSkipLessThanNppV8)
                    {
                        hasExplainedSkipLessThanNppV8 = true;
                        Npp.AddLine("Skipping UI tests and all tests that would involve reading a file, because they would cause Notepad++ versions older than v8 to crash");
                    }
                    skipped.Add(name);
                }
                else
                {
                    Npp.AddLine($@"=========================
Testing {name}
=========================
");
                    if (tester())
                        failures.Add(name);
                }
            }

            if (skipped.Count > 0)
                Npp.editor.InsertText(header.Length + 2, "Tests skipped: " + string.Join(", ", skipped) + "\r\n");
            Npp.editor.InsertText(header.Length + 2,
                failures.Count == 0 ? "No tests failed\r\n" : "Tests failed: " + string.Join(", ", failures) + "\r\n");
        }
    }
}
