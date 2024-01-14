// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    class PluginBase
    {
        internal static NppData nppData;
        internal static FuncItems _funcItems = new FuncItems();

        internal static void SetCommand(int index, string commandName, NppFuncItemDelegate functionPointer)
        {
            SetCommand(index, commandName, functionPointer, new ShortcutKey(), false);
        }

        internal static void SetCommand(int index, string commandName, NppFuncItemDelegate functionPointer, ShortcutKey shortcut)
        {
            SetCommand(index, commandName, functionPointer, shortcut, false);
        }

        internal static void SetCommand(int index, string commandName, NppFuncItemDelegate functionPointer, bool checkOnInit)
        {
            SetCommand(index, commandName, functionPointer, new ShortcutKey(), checkOnInit);
        }

        internal static void SetCommand(int index, string commandName, NppFuncItemDelegate functionPointer, ShortcutKey shortcut, bool checkOnInit)
        {
            FuncItem funcItem = new FuncItem();
            funcItem._cmdID = index;
            funcItem._itemName = commandName;
            if (functionPointer != null)
                funcItem._pFunc = new NppFuncItemDelegate(functionPointer);
            if (shortcut._key != 0)
                funcItem._pShKey = shortcut;
            funcItem._init2Check = checkOnInit;
            _funcItems.Add(funcItem);
        }

        /// <summary>
        /// if a menu item (for your plugin's drop-down menu) has a checkmark, check/uncheck it, and call its associated funcId.
        /// </summary>
        /// <param name="funcId">the index of the menu item of interest</param>
        /// <param name="isChecked">whether the menu item should be checked</param>
        public static void CheckMenuItem(int funcId, bool isChecked)
        {
            Win32.CheckMenuItem(
                Win32.GetMenu(PluginBase.nppData._nppHandle),
                PluginBase._funcItems.Items[funcId]._cmdID,
                Win32.MF_BYCOMMAND | (isChecked ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
        }

        /// <summary>
        /// if a menu item (for your plugin's drop-down menu) has a checkmark:<br></br>
        /// - if it's checked, uncheck it<br></br>
        /// - if it's unchecked, check it.
        /// Either way, call its associated funcId.
        /// </summary>
        /// <param name="funcId">the index of the menu item of interest</param>
        /// <param name="isChecked">whether the menu item is currently checked</param>
        internal static void CheckMenuItemToggle(int funcId, ref bool isCurrentlyChecked)
        {
            // toggle value
            isCurrentlyChecked = !isCurrentlyChecked;
            CheckMenuItem(funcId, isCurrentlyChecked);
        }

        internal static IntPtr GetCurrentScintilla()
        {
            int curScintilla;
            Win32.SendMessage(nppData._nppHandle, (uint) NppMsg.NPPM_GETCURRENTSCINTILLA, 0, out curScintilla);
            return (curScintilla == 0) ? nppData._scintillaMainHandle : nppData._scintillaSecondHandle;
        }


        static readonly Func<IScintillaGateway> gatewayFactory = () => new ScintillaGateway(GetCurrentScintilla());

        public static Func<IScintillaGateway> GetGatewayFactory()
        {
            return gatewayFactory;
        }
    }
}
