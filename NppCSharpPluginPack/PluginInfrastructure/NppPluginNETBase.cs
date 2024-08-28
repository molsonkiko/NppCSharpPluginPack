// NPP plugin platform for .Net v0.94.00 by Kasper B. Graversen etc.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NppDemo.Utils;

namespace Kbg.NppPluginNET.PluginInfrastructure
{
    class PluginBase
    {
        internal static NppData nppData;
        internal static FuncItems _funcItems = new FuncItems();
        private static List<string> _untranslatedFuncItemNames = new List<string>();
        public static List<string> GetUntranslatedFuncItemNames() => _untranslatedFuncItemNames.ToList();

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
            funcItem._itemName = Translator.GetTranslatedMenuItem(commandName);
            if (functionPointer != null)
                funcItem._pFunc = new NppFuncItemDelegate(functionPointer);
            if (shortcut._key != 0)
                funcItem._pShKey = shortcut;
            funcItem._init2Check = checkOnInit;
            _untranslatedFuncItemNames.Add(commandName);
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
                Win32.GetMenu(nppData._nppHandle),
                PluginBase._funcItems.Items[funcId]._cmdID,
                Win32.MF_BYCOMMAND | (isChecked ? Win32.MF_CHECKED : Win32.MF_UNCHECKED));
        }

        //private static IntPtr GetThisPluginMenuHandle()
        //{
        //    IntPtr mainMenuHandle = Win32.GetMenu(nppData._nppHandle);
        //    int pluginMenuIndex = 10;
        //    IntPtr allPluginsMenuHandle = Win32.GetSubMenu(mainMenuHandle, pluginMenuIndex);
        //    int itemCount = Win32.GetMenuItemCount(allPluginsMenuHandle);
        //    if (itemCount < 0)
        //        return IntPtr.Zero;
        //    for (int ii = 0; ii < itemCount; ii++)
        //    {
        //        var mii = new Win32.MenuItemInfo(Win32.MenuItemMask.MIIM_STRING | Win32.MenuItemMask.MIIM_SUBMENU);
        //        mii.cch = 
        //    }
        //}

        private static IntPtr MenuItemInfoToHGlobal(Win32.MenuItemInfo mii)
        {
            IntPtr miiPtr = Marshal.AllocHGlobal((int)mii.cbSize);
            Marshal.StructureToPtr(mii, miiPtr, false);
            return miiPtr;
        }

        private static bool SetMenuItemText(IntPtr hMenu, int index, string newText)
        {
            if (index >= _funcItems.Items.Count || index < 0 || newText is null)
                return false;
            var mii = new Win32.MenuItemInfo(Win32.MenuItemMask.MIIM_STRING | Win32.MenuItemMask.MIIM_FTYPE);
            IntPtr newTextPtr = Marshal.StringToHGlobalAnsi(newText);
            mii.dwTypeData = newTextPtr;
            IntPtr miiPtr = MenuItemInfoToHGlobal(mii);
            bool res = Win32.SetMenuItemInfo(hMenu, (uint)index, true, miiPtr);
            Marshal.FreeHGlobal(miiPtr);
            Marshal.FreeHGlobal(newTextPtr);
            return res;
        }

        private static unsafe bool TryGetMenuItemText(IntPtr hMenu, int index, out string str)
        {
            str = null;
            if (index < 0) // we assume the user has already checked the menu item count using Win32.GetMenuItemCount(hMenu)
                return false;
            var mii = new Win32.MenuItemInfo(Win32.MenuItemMask.MIIM_STRING | Win32.MenuItemMask.MIIM_STATE);
            IntPtr miiPtr = MenuItemInfoToHGlobal(mii);
            if (Win32.GetMenuItemInfo(hMenu, (uint)index, true, miiPtr))
            {
                mii = (Win32.MenuItemInfo)Marshal.PtrToStructure(miiPtr, typeof(Win32.MenuItemInfo));
                mii.cch++;
                byte[] textBuffer = new byte[mii.cch];
                fixed (byte * textPtr = textBuffer)
                {
                    IntPtr textPtrSafe = (IntPtr)textPtr;
                    mii.dwTypeData = textPtrSafe;
                    Marshal.StructureToPtr(mii, miiPtr, true);
                    Win32.GetMenuItemInfo(hMenu, (uint)index, true, miiPtr);
                    str = Marshal.PtrToStringAnsi(textPtrSafe);
                }
            }
            Marshal.FreeHGlobal(miiPtr);
            return !(str is null);
        }

        private static unsafe bool TryGetSubMenuWithName(IntPtr hMenu, string subMenuName, out IntPtr hSubMenu, out int idSubMenu)
        {
            idSubMenu = -1;
            hSubMenu = IntPtr.Zero;
            int itemCount = Win32.GetMenuItemCount(hMenu);
            if (itemCount < 0)
                return false;
            for (int ii = 0; ii < itemCount; ii++)
            {
                if (!TryGetMenuItemText(hMenu, ii, out string menuItemName))
                    return false;
                if (menuItemName == subMenuName)
                {
                    idSubMenu = ii;
                    hSubMenu = Win32.GetSubMenu(hMenu, ii);
                    return true;
                }
            }
            return false;
        }

        private static IntPtr _allPluginsMenuHandle = IntPtr.Zero;
        private static int _thisPluginIdxInAllPluginsMenu = -1;
        private static IntPtr _thisPluginMenuHandle = IntPtr.Zero;

        /// <summary>
        /// If allPluginsMenuHandle is a valid menu handle, and this plugin's name is the name of one of the submenus of allPluginsMenuHandle,<br></br>
        /// set _allPluginsMenuHandle to allPluginsMenuHandle, and set _thisPluginMenuHandle to the handle of the submenu with the same name as this plugin.
        /// </summary>
        /// <param name="allPluginsMenuHandle">the menu handle to the Plugins submenu of the Notepad++ main menu</param>
        /// <returns></returns>
        private static unsafe bool TrySetPluginsMenuHandle(IntPtr allPluginsMenuHandle)
        {
            if (_thisPluginMenuHandle != IntPtr.Zero && _allPluginsMenuHandle != IntPtr.Zero && _thisPluginIdxInAllPluginsMenu >= 0
                && TryGetMenuItemText(_allPluginsMenuHandle, _thisPluginIdxInAllPluginsMenu, out string pluginName)
                && pluginName == Main.PluginName)
            {
                return true;
            }
            if (!TryGetSubMenuWithName(allPluginsMenuHandle, Main.PluginName, out _thisPluginMenuHandle, out _thisPluginIdxInAllPluginsMenu))
                return false;
            _allPluginsMenuHandle = allPluginsMenuHandle;
            return true;
        }

        /// <summary>
        /// attempt to change the names of this plugin's menu items to newNames, assuming that allPluginsMenuHandle is the handle of the Plugins submenu of the Notepad++ main menu.<br></br>
        /// Returns true if and only if all the menu items could be renamed.
        /// </summary>
        /// <param name="allPluginsMenuHandle"></param>
        /// <param name="newNames"></param>
        /// <returns></returns>
        public static bool ChangePluginMenuItemNames(IntPtr allPluginsMenuHandle, List<string> newNames)
        {
            if (newNames.Count != _funcItems.Items.Count || !TrySetPluginsMenuHandle(allPluginsMenuHandle))
                return false;
            for (int ii = 0; ii < newNames.Count; ii++)
            {
                string newName = newNames[ii];
                if (newName != "---" && !SetMenuItemText(_thisPluginMenuHandle, ii, newName))
                    return false;
            }
            return true;
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
