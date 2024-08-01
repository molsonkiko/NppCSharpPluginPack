using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NppDemo.JSON_Tools;

namespace NppDemo.Utils
{
    public class SelectionManager
    {
        private static readonly Regex START_END_REGEX = new Regex(@"^\d+,\d+$", RegexOptions.Compiled);

        public static bool IsStartEnd(string x) => START_END_REGEX.IsMatch(x);

        public static (int start, int end) ParseStartEndAsTuple(string startEnd)
        {
            int[] startEndNums = ParseStartEnd(startEnd);
            return (startEndNums[0], startEndNums[1]);
        }

        public static List<(int start, int end)> GetSelectedRanges()
        {
            var selList = new List<(int start, int end)>();
            int selCount = Npp.editor.GetSelections();
            for (int ii = 0; ii < selCount; ii++)
                selList.Add((Npp.editor.GetSelectionNStart(ii), Npp.editor.GetSelectionNEnd(ii)));
            return selList;
        }

        public static bool NoTextSelected(IList<(int start, int end)> selections)
        {
            (int start, int end) = selections[0];
            return selections.Count < 2 && start == end;
        }

        public static bool NoTextSelected(IList<string> selections)
        {
            int[] startEnd = ParseStartEnd(selections[0]);
            return selections.Count < 2 && startEnd[0] == startEnd[1];
        }

        /// <summary>
        /// takes a list of one or more comma-separated integers
        /// and transforms it into an array of numbers.
        /// </summary>
        /// <param name="startEnd"></param>
        /// <returns></returns>
        public static int[] ParseStartEnd(string startEnd)
        {
            return startEnd.Split(',').Select(s => int.Parse(s)).ToArray();
        }

        public static List<(int start, int end)> SetSelectionsFromStartEnds(IEnumerable<string> startEnds)
        {
            int ii = 0;
            Npp.editor.ClearSelections();
            var result = new List<(int start, int end)>();
            var badPairs = new List<string>();
            foreach (string startEnd in startEnds)
            {
                int start, end;
                try
                {
                    (start, end) = ParseStartEndAsTuple(startEnd);
                }
                catch
                {
                    badPairs.Add(startEnd);
                    continue;
                }
                if (start > end)
                    (start, end) = (end, start);
                result.Add((start, end));
                if (ii++ == 0)
                {
                    // first selection is handled differently
                    Npp.editor.SetSelectionStart(start);
                    Npp.editor.SetSelectionEnd(end);
                }
                else
                {
                    Npp.editor.AddSelection(start, end);
                }
            }
            if (badPairs.Count > 0)
            {
                string badPairJsonStr = "[" + string.Join(", ", badPairs.Select(x => JNode.StrToString(x, true))) + "]";
                MessageBox.Show(
                    $"While setting selections from start,end integer pairs, the following integer pairs were found that had a bad format:\r\n{badPairJsonStr}",
                    "Bad start,end integer pairs when setting selections",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        /// <summary>
        /// extract INTEGER1 from string of form INTEGER1,INTEGER2
        /// </summary>
        public static int StartFromStartEnd(string s)
        {
            int commaIdx = s.IndexOf(',');
            return int.Parse(s.Substring(0, commaIdx));
        }

        /// <summary>
        /// compare two strings, "INTEGER1,INTEGER2" and "INTEGER3,INTEGER4" 
        /// comparing INTEGER1 to INTEGER3
        /// </summary>
        public static int StartEndCompareByStart(string s1, string s2)
        {
            return StartFromStartEnd(s1).CompareTo(StartFromStartEnd(s2));
        }

        public static int StartEndCompareByStart((int start, int end) se1, (int start, int end) se2)
        {
            return se1.start.CompareTo(se2.start);
        }

        /// <summary>
        /// Given selections (selstart1,selend1), (selstart2,selend2), ..., (selstartN,selendN)<br></br>
        /// returns a sep-separated list of "start,end" pairs.<br></br>
        /// EXAMPLE:<br></br>
        /// * StartEndListToString([(1, 2), (5, 7)], "], [") returns "1,2], [5,7"<br></br> 
        /// * StartEndListToString([(1, 2), (9, 20), (30,45)], " ") returns "1,2 9,20 30,45" 
        /// </summary>
        public static string StartEndListToString(IEnumerable<(int start, int end)> selections, string sep=" ")
        {
            return string.Join(sep, selections.OrderBy(x => x.start).Select(x => $"{x.start},{x.end}"));
        }

        /// <summary>
        /// equivalent to sep.Join(selections)
        /// </summary>
        public static string StartEndListToString(IEnumerable<string> selections, string sep=" ")
        {
            return string.Join(sep, selections.OrderBy(StartFromStartEnd));
        }
    }
}
