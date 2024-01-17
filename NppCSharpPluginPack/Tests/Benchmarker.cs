using System;
using System.Diagnostics;
using System.IO;
using NppDemo.Utils;

namespace NppDemo.Tests
{
    /// <summary>
    /// put performance benchmarks for your code in this file (assuming you want to run them with TestRunner.cs)
    /// </summary>
    public class Benchmarker
    {
        /// <summary>
        /// Repeatedly run tests on the text of a file.<br></br>
        /// For the most recent benchmarking results, see "most recent errors.txt" in the main repository.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="num_parse_trials"></param>
        public static bool BenchmarkSomethingThatReadsAFile((string query, string description)[] queriesAndDescriptions, 
                                     string fname,
                                     string benchmarkDescription,
                                     int numTrials)
        {
            // setup
            Stopwatch watch = new Stopwatch();
            if (!File.Exists(fname))
            {
                Npp.AddLine($"Can't run benchmark tests because file {fname}\ndoes not exist");
                return true;
            }
            string filestr = File.ReadAllText(fname);
            int len = filestr.Length;
            foreach ((string query, string description) in queriesAndDescriptions)
            {
                Npp.AddLine($@"=========================
Performance tests for {benchmarkDescription} ({description})
=========================
");
                // time query execution
                long[] queryTimes = new long[numTrials];
                for (int ii = 0; ii < numTrials; ii++)
                {
                    watch.Reset();
                    watch.Start();
                    try
                    {
                        // do a thing
                    }
                    catch (Exception ex)
                    {
                        Npp.AddLine($"Couldn't run {benchmarkDescription} benchmarks because of error while executing compiled query:\n{ex}");
                        return true;
                    }
                    watch.Stop();
                    long t = watch.Elapsed.Ticks;
                    queryTimes[ii] = t;
                }
                // display querying results
                (double mean, double sd) = GetMeanAndSd(queryTimes);
                Npp.AddLine($"To run query \"{query}\" on file of size {len} into took {ConvertTicks(mean)} +/- {ConvertTicks(sd)} ms over {numTrials} trials");
                var queryTimesStr = new string[queryTimes.Length];
                for (int ii = 0; ii < queryTimes.Length; ii++)
                {
                    queryTimesStr[ii] = Math.Round(queryTimes[ii] / 1e4, 3).ToString();
                }
                Npp.AddLine($"Query times (ms): {String.Join(", ", queryTimesStr)}");
                string resultPreview = "Preview of result";
                Npp.AddLine($"Preview of result: {resultPreview}");
            }
            return false;
        }

        public static (double mean, double sd) GetMeanAndSd(long[] times)
        {
            double mean = 0;
            foreach (long t in times) { mean += t; }
            mean /= times.Length;
            double sd = 0;
            foreach (long t in times)
            {
                double diff = t - mean;
                sd += diff * diff;
            }
            sd = Math.Sqrt(sd / times.Length);
            return (mean, sd);
        }

        public static double ConvertTicks(double ticks, string newUnit = "ms", int sigfigs = 3)
        {
            switch (newUnit)
            {
                case "ms": return Math.Round(ticks / 1e4, sigfigs);
                case "s": return Math.Round(ticks / 1e7, sigfigs);
                case "ns": return Math.Round(ticks * 100, sigfigs);
                case "mus": return Math.Round(ticks / 10, sigfigs);
                default: throw new ArgumentException("Time unit must be s, mus, ms, or ns");
            }
        }
    }
}
