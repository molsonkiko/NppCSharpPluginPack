using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NppDemo.JSON_Tools;
using Kbg.NppPluginNET;
using Kbg.NppPluginNET.PluginInfrastructure;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NppDemo.Utils
{
    public static class Translator
    {
        private static JObject translations = null;
        public static bool HasTranslations => !(translations is null);

        public static bool HasLoadedAtStartup { get; private set; } = false;

        public static string languageName { get; private set; } = DEFAULT_LANG;

        public const string DEFAULT_LANG = "english";
        public const string DEFAULT_PLUGINS_MENU_NAME = "&Plugins";
        public static readonly string translationDir = Path.Combine(Npp.pluginDllDirectory, "translation");

        /// <summary>
        /// Attempts to load translations from a config file.
        /// </summary>
        /// <param name="atStartup">Is this method being called as the plugin is starting up?</param>
        /// <param name="preferredLang">If null, use the logic described in README.md to determine which language to load. Otherwise, do not attempt to load any language other than preferredLang.</param>
        private static void LoadTranslations(bool atStartup = true)
        {
            if (atStartup && languageName == DEFAULT_LANG)
            {
                //MessageBox.Show("Not loading translations, because english is the current culture language");
                return;
            }
            if (!TryGetTranslationFileName(languageName, out string translationFilename))
            {
                //MessageBox.Show($"Could not find a translation file for language {languageFirstname} in directory {translationDir}");
                languageName = DEFAULT_LANG;
                return;
            }
            FileInfo translationFile = new FileInfo(translationFilename);
            try
            {
                var parser = new JsonParser(LoggerLevel.JSON5);
                string translationFileText;
                using (var fp = new StreamReader(translationFile.OpenRead(), Encoding.UTF8, true))
                {
                    translationFileText = fp.ReadToEnd();
                }
                translations = (JObject)parser.Parse(translationFileText);
                PropagateGlobalConstants();
                //MessageBox.Show($"Found and successfully parsed translation file at path {translationFilename}");
            }
            catch (Exception ex)
            {
                languageName = DEFAULT_LANG;
                MessageBox.Show($"While attempting to parse translation file {translationFilename}, got an exception:\r\n{ex}");
            }
        }

        /// <summary>
        /// Attempts to read the nativeLang.xml file that determines language of Notepad++ UI. If successful, returns true and sets nativeLangXml to text of that file.<br></br>
        /// On failure, nativeLangXml = null and returns false
        /// </summary>
        /// <param name="nativeLangXml"></param>
        /// <returns></returns>
        private static bool TryGetNppNativeLangXmlText(out string nativeLangXml)
        {
            string nativeLangXmlFname = Path.Combine(Npp.notepad.GetConfigDirectory(), "..", "..", "nativeLang.xml");
            if (File.Exists(nativeLangXmlFname))
            {
                try
                {
                    using (var reader = new StreamReader(File.OpenRead(nativeLangXmlFname), Encoding.UTF8, true))
                    {
                        nativeLangXml = reader.ReadToEnd();
                    }
                    return true;
                }
                catch //(Exception ex)
                {
                    //MessageBox.Show($"While attempting to determine native language preference from Notepad++ config XML, got an error:\r\n{ex}");
                }
            }
            nativeLangXml = null;
            return false;
        }

        private static bool TryGetTranslationFileName(string langName, out string translationFilename)
        {
            translationFilename = Path.Combine(translationDir, langName + ".json5");
            if (!File.Exists(translationFilename))
            {
                translationFilename = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Find the string at the end of a sequence of keys in translations, returning false if no such string was found.<br></br>
        /// For example:<br></br>
        /// <example>
        /// Suppose translations is
        /// <code>{"foo": {"bar": "1", "rnq": "2"}, "quz": "3"}</code>
        /// - <c>TryGetTranslationAtPath(["foo", "bar"], out JNode result)</c> would set result to <c>"1"</c> and return <c>true</c><br></br>
        /// - <c>TryGetTranslationAtPath(["foo", "rnq"], out JNode result)</c> would set result to <c>"2"</c> and return <c>true</c><br></br>
        /// - <c>TryGetTranslationAtPath(["blah", "rnq"], out JNode result)</c> would set result to <c>null</c> and return <c>false</c><br></br>
        /// - <c>TryGetTranslationAtPath(["foo", "rnq", "b"], out JNode result)</c> would set result to <c>null</c> and return <c>false</c><br></br>
        /// - <c>TryGetTranslationAtPath(["foo", "b"], out JNode result)</c> would set result to <c>null</c> and return <c>false</c><br></br>
        /// - <c>TryGetTranslationAtPath(["quz"], out JNode result)</c> would set result to <c>"3"</c> and return <c>true</c><br></br>
        /// - <c>TryGetTranslationAtPath(["foo"], out JNode result)</c> would set result to <c>{"bar": "1", "rnq": "2"}</c> and return <c>true</c><br></br>
        /// </example>
        /// </summary>
        /// <param name="pathToTrans"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetTranslationAtPath(string[] pathToTrans, out JNode result)
        {
            result = null;
            if (!(translations is JObject trans) || pathToTrans.Length == 0)
                return false;
            int pathLen = pathToTrans.Length;
            JNode child = new JNode();
            for (int ii = 0; ii < pathLen; ii++)
            {
                string key = pathToTrans[ii];
                if (!trans.TryGetValue(key, out child))
                    return false;
                if (ii < pathLen - 1)
                {
                    if (child is JObject childObj)
                        trans = childObj;
                    else
                        return false;
                }
            }
            result = child;
            return true;
        }

        private static readonly Regex constantNameRegex = new Regex(@"\A\$[a-zA-Z_]+\$\z", RegexOptions.Compiled);

        /// <summary>
        /// Replaces all instances of constant names (see the "$constants$" field of the translation files) with their value in the translation file
        /// </summary>
        private static void PropagateGlobalConstants()
        {
            if (translations is JObject jobj
                && translations.children.TryGetValue("$constants$", out JNode constsNode) && constsNode is JObject consts)
            {
                var constants = new Dictionary<string, string>();
                var invalidConstantNames = new JArray();
                foreach (string key in consts.children.Keys)
                {
                    if (constantNameRegex.IsMatch(key))
                        constants[key] = consts[key].ValueOrToString();
                    else
                        invalidConstantNames.children.Add(new JNode(key));
                }
                if (invalidConstantNames.Length > 0)
                {
                    MessageBox.Show($"The \"$constants$\" field for the {languageName}.json5 file contains constants where the key does not match the regular expression {JNode.StrToString(constantNameRegex.ToString(), true)}:\r\n{invalidConstantNames.ToString()}",
                        "Invalid \"$constants$\" field", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (constants.Count > 0)
                {
                    foreach (KeyValuePair<string, JNode> kv in jobj.children)
                    {
                        if (kv.Key != "$constants$")
                            PropagateGlobalConstantsHelper(kv.Value, constants);
                    }
                }
            }
        }

        private static void PropagateGlobalConstantsHelper(JNode node, Dictionary<string, string> constants)
        {
            if (node.value is string s && s.IndexOf('$') >= 0)
            {
                node.value = PropagateConstantsToString(s, constants);
            }
            else if (node is JArray jarr)
            {
                foreach (JNode child in jarr.children)
                    PropagateGlobalConstantsHelper(child, constants);
            }
            else if (node is JObject jobj)
            {
                foreach (JNode child in jobj.children.Values)
                    PropagateGlobalConstantsHelper(child, constants);
                var keys = jobj.children.Keys.ToArray();
                foreach (string key in keys)
                {
                    if (key.IndexOf('$') >= 0)
                    {
                        string newKey = PropagateConstantsToString(key, constants);
                        if (newKey != key)
                        {
                            jobj[newKey] = jobj[key];
                            jobj.children.Remove(key);
                        }
                    }
                }
            }
        }

        private static string PropagateConstantsToString(string s, Dictionary<string, string> constants)
        {
            string newValue = s;
            foreach (KeyValuePair<string, string> kv in constants)
                newValue = newValue.Replace(kv.Key, kv.Value);
            return newValue;
        }

        /// <summary>
        /// Translates menu item values (using <paramref name="menuItem"/> as the key in the <c>"menuItems"</c> field)
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        public static string GetTranslatedMenuItem(string menuItem)
        {
            if (translations is JObject jobj
                && jobj.TryGetValue("menuItems", out JNode menuItemsObjNode)
                && menuItemsObjNode is JObject menuItemsObj
                && menuItemsObj.TryGetValue(menuItem, out JNode menuItemNode)
                && menuItemNode.value is string s)
            {
                if (s.Length > FuncItem.MAX_FUNC_ITEM_NAME_LENGTH)
                {
                    MessageBox.Show($"The translation {JNode.StrToString(s, true)} has {s.Length} characters, " +
                        $"but it must have {FuncItem.MAX_FUNC_ITEM_NAME_LENGTH} or fewer characters.\r\n" +
                        $"Because of this, the untranslated command, {JNode.StrToString(menuItem, true)} is being used.",
                        "Too many characters in menu item translation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return menuItem;
                }
                return s;
            }
            return menuItem;
        }

        #region Reloading translations
        /// <summary>
        /// When the UI language of Notepad++ changes, attempt to translate JsonTools to the Notepad++ UI language automatically.<br></br>
        /// If translation fails, or if the current UI language is english, restore the language of JsonTools to the default english.
        /// </summary>
        /// <returns>true if and only if translation was successful AND the plugin was translated to a language other than English</returns>
        public static bool ResetTranslations(bool atStartup)
        {
            if (atStartup && HasLoadedAtStartup)
                return false;
            HasLoadedAtStartup = true;
            string oldLanguageName = languageName;
            languageName = DEFAULT_LANG;
            string newLanguageName = "";
            // first try using the NPPM_GETNATIVELANGNAME API introduced in v8.7.0
            if (Npp.notepad.TryGetNativeLangName(out newLanguageName))
            {
                newLanguageName = GetEmptyStringIfLangInvalid(newLanguageName, atStartup);
            }
            else
            {
                // if that failed, try to use the Notepad++ nativeLang.xml config file to determine the user's language preference
                if (TryGetNppNativeLangXmlText(out string nativeLangXml))
                {
                    Match langNameMtch = Regex.Match(nativeLangXml, "<Native-Langue .*? filename=\"(.*?)\\.xml\"");
                    if (langNameMtch.Success)
                        newLanguageName = langNameMtch.Groups[1].Value.Trim().ToLower();
                    newLanguageName = GetEmptyStringIfLangInvalid(newLanguageName, atStartup);
                }
            }
            // at startup only, if neither of the above worked, try to determine the user's language by asking Windows for their current culture
            // Doing this any time other than startup can have confusing consequences
            if (atStartup && newLanguageName.Length == 0)
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                string languageFullname = currentCulture.EnglishName;
                newLanguageName = languageFullname.Split(' ')[0].ToLower();
                if (newLanguageName == "Unknown")
                    newLanguageName = currentCulture.Parent.EnglishName.Split(' ')[0].ToLower();
                if (!TryGetTranslationFileName(newLanguageName, out _))
                    newLanguageName = DEFAULT_LANG;
                newLanguageName = GetEmptyStringIfLangInvalid(newLanguageName, atStartup);
            }
            if (newLanguageName.Length == 0 || newLanguageName == oldLanguageName)
            {
                // the new language is invalid or unchanged, so we do nothing
                languageName = oldLanguageName;
                return false;
            }
            languageName = newLanguageName;
            return ResetTranslationsHelper(languageName == DEFAULT_LANG, atStartup);
        }

        private static string GetEmptyStringIfLangInvalid(string langName, bool atStartup)
        {
            return ((atStartup && langName == DEFAULT_LANG) || !TryGetTranslationFileName(langName, out _))
                ? "" // not a valid language, or it's English and we're starting up so no translation needed
                : langName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="restoreToEnglish"></param>
        /// <returns>true if and only if translation was successful AND the plugin was translated to a language other than English</returns>
        private static bool ResetTranslationsHelper(bool restoreToEnglish, bool atStartup)
        {
            bool result = true;
            List<string> newMenuItemNames = PluginBase.GetUntranslatedFuncItemNames();
            LoadTranslations(atStartup);
            if (restoreToEnglish)
                result = false;
            else
                newMenuItemNames = newMenuItemNames.Select(x => GetTranslatedMenuItem(x)).ToList();
            IntPtr allPluginsMenuHandle = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETMENUHANDLE, (int)NppMsg.NPPPLUGINMENU, 0);
            if (allPluginsMenuHandle == IntPtr.Zero)
                result = false;
            else if (!(atStartup && restoreToEnglish))
                PluginBase.ChangePluginMenuItemNames(allPluginsMenuHandle, newMenuItemNames);
            if (Main.selectionRememberingForm != null && !Main.selectionRememberingForm.IsDisposed)
            {
                TranslateForm(Main.selectionRememberingForm);
            }
            if (restoreToEnglish) // now that we're done restoring forms to English, we can reset translations to null
                translations = null;
            return result;
        }

        #endregion // translating menu items

        /// <summary>
        /// Finds the appropriate translation for a <see cref="MessageBox"/> (using <paramref name="caption"/> as key in the <c>"messageBoxes"</c> field), then displays it and returns the result of <see cref="MessageBox.Show(string, string, MessageBoxButtons, MessageBoxIcon)"/><br></br>
        /// <b>EXAMPLE 1:</b><br></br>
        /// Suppose the <c>"messageBoxes"</c> field of your active translation file looks like this:
        /// <code>
        /// {
        ///     "Foo is not compatible with {0}": {
        ///         "caption": "Foo no es compatible con {0}",
        ///         "text": "Cuando foo era {0}, esperaba que {1} estará {2}, pero {1} era {3}"
        ///     }
        /// }
        /// </code>
        /// - <c>Translator.ShowTranslatedMessageBox("When foo was {0}, expected that {1} would be {2}, but {1} was {3}", "Foo is not compatible with {0}", MessageBoxButtons.OK, MessageBoxIcon.Warning, 4, "fooValue", "bar", "barExpectedValue", "barActualValue", "bar");</c> would show a MessageBox with caption <c>"Foo no es compatible con bar"</c> and text <c>"Cuando foo era fooValue, esperaba que bar estará barExpectedValue, pero bar era barActualValue"</c> (and the <c>OK</c> button), and would return <see cref="DialogResult.OK"/>.<br></br>
        /// <b>EXAMPLE 2:</b><br></br>
        /// Suppose that there is <b>no active translation file</b> (so everything is in the original English)<br></br>
        /// - <c>Translator.ShowTranslatedMessageBox("When foo was {0}, expected that {1} would be {2}, but {1} was {3}", "Foo is not compatible with {0}", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 4, "fooValue", "quz", "quzExpectedValue", "quzActualValue", "QUZQUZ");</c> would show a MessageBox with caption <c>"Foo is not compatible with QUZQUZ"</c> and text <c>"When foo was fooValue, expected that quz would be quzExpectedValue, but quz was quzActualValue"</c> (and <c>Yes</c> and <c>No</c> buttons), and would return <see cref="DialogResult.Yes"/> if the user clicked <c>Yes</c>
        /// </summary>
        /// <param name="text">the text of the MessageBox, which may be a format string of the form <c>"expected {0}, got {1}"</c></param>
        /// <param name="caption">the text of the MessageBox, which may be a format string of the form <c>"expected {0}, got {1}"</c></param>
        /// <param name="buttons">which buttons to show in the MB</param>
        /// <param name="icon">the icon to use</param>
        /// <param name="nTextParams">the first nTextParams values in the <paramref name="formattingParams"/> are used to format the <paramref name="text"/> argument</param>
        /// <param name="nCaptionParams">the first nTextParams values in the <paramref name="formattingParams"/> are used to format the <paramref name="text"/> argument</param>
        /// <param name="formattingParams">all values in this array after the first <paramref name="nTextParams"/> are used to format the caption</param>
        /// <returns>the result of MessageBox.Show with the translated box</returns>
        public static DialogResult ShowTranslatedMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, int nTextParams = 0, params object[] formattingParams)
        {
            string translatedText = text, translatedCaption = caption;
            if (translations is JObject jobj
                && jobj.TryGetValue("messageBoxes", out JNode messagesNode) && messagesNode is JObject messages
                && messages.TryGetValue(caption, out JNode mbTransNode) && mbTransNode is JObject mbTrans
                && mbTrans.children is Dictionary<string, JNode> mbDict)
            {
                if (mbDict.TryGetValue("caption", out JNode captionTrans) && captionTrans.value is string captionTransStr)
                    translatedCaption = captionTransStr;
                if (mbDict.TryGetValue("text", out JNode textTrans) && textTrans.value is string textTransStr)
                    translatedText = textTransStr;
            }
            if (formattingParams.Length < nTextParams)
            {
                MessageBox.Show($"While attempting to call ShowTranslatedMessageBox({JNode.StrToString(text, true)}, {JNode.StrToString(caption, true)}, {buttons}, {icon}, {nTextParams}, ...)\r\ngot {formattingParams.Length} formatting params, but expected {nTextParams}", "error in formatting for ShowTranslatedMessageBox", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return MessageBox.Show(translatedText, translatedCaption, buttons, icon);
            }
            int nCaptionParams = formattingParams.Length - nTextParams;
            object[] textParams = formattingParams.Take(nTextParams).ToArray(), captionParams = formattingParams.LazySlice(nTextParams).ToArray();
            string formattedText = nTextParams == 0 ? translatedText : TryTranslateWithFormatting(text, translatedText, textParams);
            string formattedCaption = nCaptionParams == 0 ? translatedCaption : TryTranslateWithFormatting(caption, translatedCaption, captionParams);
            return MessageBox.Show(formattedText, formattedCaption, buttons, icon);
        }

        public static string TryTranslateWithFormatting(string untranslated, string translated, params object[] formatParams)
        {
            try
            {
                return string.Format(translated, formatParams);
            }
            catch { }
            try
            {
                return string.Format(untranslated, formatParams);
            }
            catch
            {
                return untranslated;
            }
        }

        /// <summary>
        /// Used to translate the settings in <see cref="Settings"/>.<br></br>
        /// If there is no active translation file, return the propertyInfo's <see cref="DescriptionAttribute.Description"/>
        /// (which can be seen in the source code in the Description decorator of each setting).<br></br>
        /// If <paramref name="propertyInfo"/>.Name is a field in the <c>"settingsDescriptions"</c> field of the active translation file, return that value.
        /// </summary>
        public static string TranslateSettingsDescription(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null)
                return "";
            if (translations is JObject jobj
                && jobj.TryGetValue("settingsDescriptions", out JNode settingsDescNode)
                && settingsDescNode is JObject settingsDescObj
                && settingsDescObj.TryGetValue(propertyInfo.Name, out JNode descNode)
                && descNode.value is string s)
            {
                return s;
            }
            if (propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute description)
                return description.Description;
            return "";
        }

        #region Form translation

        /// <summary>
        /// This assumes that each character in Size 7.8 font is 7 pixels across.<br></br>
        /// In practice, this generally returns a number that is greater than the width of the text.
        /// </summary>
        private static int WidthInCharacters(int numChars, float fontSize)
        {
            return Convert.ToInt32(Math.Round(numChars * fontSize / 7.8 * 7));
        }

        /// <summary>
        /// translate a form and its controls 
        /// according to the entry corresponding to the form's name in the <c>"forms"</c> field of the translations object.
        /// </summary>
        public static void TranslateForm(Form form)
        {
            if (translations is JObject jobj
                && jobj.TryGetValue("forms", out JNode formsNode)
                && formsNode is JObject allFormsObj
                && allFormsObj.TryGetValue(form.Name, out JNode formNode)
                && formNode is JObject formObj && formObj.children is Dictionary<string, JNode> formDict)
            {
                if (formDict.TryGetValue("title", out JNode transTitle) && transTitle.value is string transStr)
                    form.Text = transStr;
                if (formDict.TryGetValue("controls", out JNode controlTransNode)
                    && controlTransNode is JObject controlTranslationsObj
                    && controlTranslationsObj.children is Dictionary<string, JNode> controlTranslations)
                {
                    TranslateControl(form, controlTranslations);
                    foreach (Form childForm in form.OwnedForms)
                        TranslateForm(childForm);
                }
            }
        }

        /// <summary>
        /// Translate a control according to the logic described in "/translations/english.json5" (relative to the repo root)<br></br>
        /// and recursively translate any child controls.
        /// </summary>
        /// <param name="controlTranslations">The translations for all the controls in the form</param>
        private static void TranslateControl(Control ctrl, Dictionary<string, JNode> controlTranslations)
        {
            if (!(ctrl is Form) && controlTranslations.TryGetValue(ctrl.Name, out JNode ctrlNode))
            {
                if (ctrlNode.value is string s)
                    ctrl.Text = s;
                else if (ctrlNode is JObject ctrlObj && ctrlObj.children is Dictionary<string, JNode> ctrlDict)
                {
                    if (ctrl is CheckBox checkBox
                        && ctrlDict.TryGetValue("checked", out JNode checkedNode) && checkedNode.value is string checkedVal
                        && ctrlDict.TryGetValue("unchecked", out JNode uncheckedNode) && uncheckedNode.value is string uncheckedVal)
                    {
                        checkBox.Text = checkBox.Checked ? checkedVal : uncheckedVal;
                        checkBox.CheckedChanged += (object sender, EventArgs e) =>
                        {
                            checkBox.Text = checkBox.Checked ? checkedVal : uncheckedVal;
                        };
                    }
                    else if (ctrl is LinkLabel llbl
                        && ctrlDict.TryGetValue("text", out JNode textNode) && textNode.value is string textVal
                        && ctrlDict.TryGetValue("linkStart", out JNode linkStartNode) && linkStartNode.value is long linkStartVal
                        && ctrlDict.TryGetValue("linkLength", out JNode linkLengthNode) && linkLengthNode.value is long linkLengthVal)
                    {
                        llbl.Text = textVal;
                        llbl.LinkArea = new LinkArea(Convert.ToInt32(linkStartVal), Convert.ToInt32(linkLengthVal));
                    }
                }
                else if (ctrlNode is JArray ctrlArr && ctrlArr.children is List<JNode> ctrlList && ctrl is ComboBox comboBox)
                {
                    int maxTranslationLen = 0;
                    for (int ii = 0; ii < comboBox.Items.Count && ii < ctrlList.Count; ii++)
                    {
                        JNode translationII = ctrlList[ii];
                        if (translationII.value is string translationVal)
                        {
                            comboBox.Items[ii] = translationVal;
                            if (translationVal.Length > maxTranslationLen)
                                maxTranslationLen = translationVal.Length;
                        }
                    }
                    int newDropDownWidth = WidthInCharacters(maxTranslationLen, ctrl.Font.Size);
                    if (newDropDownWidth > comboBox.DropDownWidth)
                        comboBox.DropDownWidth = newDropDownWidth;
                }
            }
            int ctrlCount = ctrl.Controls.Count;
            if (ctrlCount > 0)
            {
                // translate each child of this control
                var childrenByLeft = new List<(Control ctrl, bool textWasMultiline, bool isAnchoredRight)>(ctrlCount);
                foreach (Control child in ctrl.Controls)
                {
                    int childWidthInChars = WidthInCharacters(child.Text.Length, child.Font.Size);
                    bool textWasMultiline = !child.AutoSize && ((child is Label || child is LinkLabel) && childWidthInChars > child.Width);
                    TranslateControl(child, controlTranslations);
                    childrenByLeft.Add((child, textWasMultiline, (child.Anchor & AnchorStyles.Right) == AnchorStyles.Right));
                }
                // the child controls may have more text now,
                //    so we need to resolve any collisions that may have resulted.
                childrenByLeft.Sort((x, y) => x.ctrl.Left.CompareTo(y.ctrl.Left));
                for (int ii = 0; ii < ctrlCount; ii++)
                {
                    (Control child, bool textWasMultiline, bool isAnchoredRight) = childrenByLeft[ii];
                    int textLen = child.Text.Length;
                    int widthIfButtonOrLabel = WidthInCharacters(textLen, child.Font.Size + 1);
                    if (child is Button && textLen > 2 && textLen < 14)
                    {
                        int increase = textLen <= 5 ? 25 : (15 - textLen) * 2 + 7;
                        widthIfButtonOrLabel += increase;
                    }
                    if (!textWasMultiline && (child is Button || child is Label || child is CheckBox) &&
                        !child.Text.Contains('\n') && widthIfButtonOrLabel > child.Width)
                    {
                        child.Width = widthIfButtonOrLabel;
                        if (isAnchoredRight)
                            child.Left -= 5; // to account for weirdness when you resize a right-anchored thing
                        // if there are overlapping controls to the right, move those far enough right that they don't overlap.
                        for (int jj = ii + 1; jj < ctrlCount; jj++)
                        {
                            Control otherChild = childrenByLeft[jj].ctrl;
                            if (otherChild.Left > child.Left && Overlap(child, otherChild))
                            {
                                if (otherChild.Left > child.Left)
                                    otherChild.Left = child.Right + 2;
                            }
                        }
                    }
                    child.Refresh();
                }
                int maxRight = 0;
                foreach (Control child in ctrl.Controls)
                    maxRight = child.Right > maxRight ? child.Right : maxRight;
                // Temporarily turn off the normal layout logic,
                //     because this would cause controls to move right when the form is resized
                //     (yes, even if they're not anchored right. No, that doesn't make sense)
                ctrl.SuspendLayout();
                if (maxRight > ctrl.Width)
                {
                    int padding = ctrl is Form ? 25 : 5;
                    ctrl.Width = maxRight + padding;
                }
                // Resume layout logic, ignoring the pending requests to move controls right due to resizing of form
                ctrl.ResumeLayout(false);
            }
        }

        private static bool VerticalOverlap(Control ctrl1, Control ctrl2)
        {
            return !(ctrl1.Bottom < ctrl2.Top || ctrl2.Bottom < ctrl1.Top);
        }

        private static bool HorizontalOverlap(Control ctrl1, Control ctrl2)
        {
            return !(ctrl1.Right < ctrl2.Left || ctrl2.Right < ctrl1.Left);
        }

        private static bool Overlap(Control ctrl1, Control ctrl2)
        {
            return HorizontalOverlap(ctrl1, ctrl2) && VerticalOverlap(ctrl1, ctrl2);
        }
        #endregion
    }
}
