/**
 * @license
 * Internet Systems Consortium license
 *
 * Copyright (c) 2020 Maksym Sadovnychyy (MAKS-IT)
 * Website: https://maks-it.com
 * Email: commercial@maks-it.com
 *
 * Permission to use, copy, modify, and/or distribute this software for any purpose
 * with or without fee is hereby granted, provided that the above copyright notice
 * and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
 * REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
 * INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
 * OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
 * TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF
 * THIS SOFTWARE.
 */

using System;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.IO;

namespace Lib
{
    /*
        The main <c>Shortcuts</c> class
        Contains all methods for performing basic actions with Shortcuts
    */
    /// <summary>
    /// The main <c>Shortcuts</c> class.
    /// Contains all methods for performing basic actions with Shortcuts.
    /// <list type="bullet">
    /// <item>
    /// <term>Create</term>
    /// <description>Creates new shortcut</description>
    /// </item>
    /// <item>
    /// <term>Change</term>
    /// <description>Changes existing shortcut</description>
    /// </item>
    /// <item>
    /// <term>Delete</term>
    /// <description>Deletes existing shortcut</description>
    /// </item>
    /// <item>
    /// <term>PinToStart</term>
    /// <description>Pins or unpins shortcut to Start Menu on Windows 7, 8, 8.1, 10</description>
    /// </item>
    /// <item>
    /// <term>PinToTaskBar</term>
    /// <description>Pins or unpins shortcut to the Task Bar on Windows 7, 8, 8.1 10 (unpin not implementd)</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>This class cann create, change and delete shortcuts.</para>
    /// <para>Also are implemented non documented functionalities Pin to Start and Pin to Task Bar</para>
    /// </remarks>
    public static class CMShortcuts
    {
        // Allows to create shortcut
        /// <summary>
        /// Allows to create shortcut by providing <paramref name="shortcutPath"/>, <paramref name="targetPath"/> and optional parameters
        /// <paramref name="arguments"/> and <paramref name="description"/>
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <param name="targetPath"></param>
        /// <param name="arguments"></param>
        /// <param name="description"></param>
        public static void Create(string shortcutPath, string targetPath, string arguments = "", string description = "")
        {
            if (!System.IO.File.Exists(shortcutPath))
            {
                WshShell shell = new WshShell();

                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = description;
                shortcut.TargetPath = targetPath;
                shortcut.Arguments = arguments;
                shortcut.Save();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchPath"></param>
        /// <param name="shortcutName"></param>
        /// <param name="targetPath"></param>
        /// <param name="arguments"></param>
        /// <param name="description"></param>
        public static void Change(string searchPath, string shortcutName, string targetPath = null, string arguments = null, string description = null)
        {
            if (System.IO.File.Exists(searchPath))
            {
                foreach (FileInfo file in new DirectoryInfo(searchPath).GetFiles())
                {
                    if (file.Name.ToLower().Contains(shortcutName.ToLower()))
                    {
                        WshShell shell = new WshShell();

                        IWshShortcut shortcut = shell.CreateShortcut(file.FullName);
                        if (description != null)
                            shortcut.Description = description;

                        if (targetPath != null)
                            shortcut.TargetPath = targetPath;

                        if (arguments != null)
                            shortcut.Arguments = arguments;

                        shortcut.Save();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchPath"></param>
        /// <param name="shortcutName"></param>
        /// <param name="targetPath"></param>
        /// <param name="arguments"></param>
        /// <param name="description"></param>
        public static void Delete(string searchPath, string shortcutName, string targetPath = null, string arguments = null, string description = null) {
            if (System.IO.File.Exists(searchPath))
            {
                foreach (FileInfo file in new DirectoryInfo(searchPath).GetFiles())
                {
                    if (Path.GetExtension(file.Name) == ".lnk")
                    {
                        WshShell shell = new WshShell();

                        IWshShortcut shortcut = shell.CreateShortcut(file.FullName);
                        if (Path.GetFileNameWithoutExtension(shortcut.FullName).ToLower().Contains(shortcutName.ToLower()) && shortcut.TargetPath.ToLower().Contains(targetPath.ToLower()) && shortcut.Arguments.ToLower().Contains(arguments.ToLower()))
                        {
                            System.IO.File.Delete(shortcut.FullName);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Main <c>Shortcut</c> class.
    /// This class produces Shortcut object to be used with <c>ShortcutExtension</c> class.
    /// <seealso cref="ShotrtcutExtension"/>
    /// <example>
    /// <c>Shortcut shortcut = new Shortcut(@"C:\Users\Username\Desktop\Chrome.lnk");</c>
    /// </example>
    /// </summary>
    public class Shortcut
    {
        /// <summary>
        /// 
        /// </summary>
        public string ShortcutPath { get; }
        /// <summary>
        /// 
        /// </summary>
        public dynamic Item { get; }

        /// <summary>
        /// <c>Shortcut</c> class constructor.
        /// </summary>
        /// <param name="shortcutPath"></param>
        public Shortcut(string shortcutPath)
        {
            ShortcutPath = shortcutPath;

            // create the shell application object
            dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));

            dynamic folder = shell.Namespace(Directory.GetParent(shortcutPath).FullName);
            Item = folder.Parsename(Path.GetFileName(shortcutPath));
        }
    }

    /// <summary>
    /// <c>ShotrtcutExtension</c> class for <c>Shortcut</c> object.
    /// <list type="bullet">
    /// <item>
    /// <term>PinToTaskBar</term>
    /// <description>Allows to pin shortcut to the task bar</description>
    /// </item>
    /// <item>
    /// <term>UnpinFromTaskBar</term>
    /// <description>Allows to unpin shortcut from the task bar. (Not working on windows 10)</description>
    /// </item>
    /// <item>
    /// <term>PinToStartMenu</term>
    /// <description>Allows to pin shortcut to the start menu</description>
    /// </item>
    /// <item>
    /// <term>UnpinFromStartMenu</term>
    /// <description>Allows to unpin shortcut from the start menu</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <para>This extension class pin and unpin shortcut to start menu or task bar</para>
    public static class ShotrtcutExtension
    {
        private static string currentVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", "").ToString();

        /// <summary>
        /// Pins shortcut to the taskbar
        /// </summary>
        /// <example>
        /// <code>
        /// Shortcut shortcut = new Shortcut("path to shortcut");
        /// shortcut.PinToStart();
        /// </code>
        /// </example>
        /// <seealso cref="DoVerb"/>
        /// <param name="shortcut"></param>
        public static void PinToTaskBar(this Shortcut shortcut)
        {
            if (currentVersion == "6.1")
            {
                DoVerb(shortcut.Item, "Pin to Tas&kbar");
            }

            if (currentVersion == "6.3")
            {
                string keyPath1 = "shell";
                string keyPath2 = "{:}";
                string valueName = "ExplorerCommandHandler";
                string valueData = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\Windows.taskbarpin", "ExplorerCommandHandler", "").ToString();

                RegistryKey key2 = CMWinRegistry.RegKeyFromString(@"HKLM_CURRENT_USER\SOFTWARE\Classes\*", true);
                RegistryKey key3 = key2.CreateSubKey("shell");
                RegistryKey key4 = key3.CreateSubKey("{:}");
                key4.SetValue(valueName, valueData);

                shortcut.Item.InvokeVerb("{:}");

                key3.DeleteSubKey(keyPath2);
                if (key3.SubKeyCount == 0 && key3.ValueCount == 0)
                {
                    key2.DeleteSubKey(keyPath1);
                }
            }
        }

        /// <summary>
        /// Unpins shortcut from the taskbar
        /// </summary>
        /// <example>
        /// <code>
        /// Shortcut shortcut = new Shortcut("path to shortcut");
        /// shortcut.UnpinFromTaskBar();
        /// </code>
        /// </example>
        /// <seealso cref="DoVerb"/>
        /// <param name="shortcut"></param>
        public static void UnpinFromTaskBar(this Shortcut shortcut)
        {
            if (currentVersion == "6.1")
            {
                DoVerb(shortcut.Item, "Unpin from Tas&kbar");
            }

            if (currentVersion == "6.3")
            {

            }
        }

        /// <summary>
        /// Pins shortcut to the start menu
        /// </summary>
        /// <example>
        /// <code>
        /// Shortcut shortcut = new Shortcut("path to shortcut");
        /// shortcut.PinToStartMenu();
        /// </code>
        /// </example>
        /// <seealso cref="DoVerb"/>
        /// <param name="shortcut"></param>
        public static void PinToStartMenu(this Shortcut shortcut)
        {
            string verbText = "";
            if (currentVersion.ToLower().Contains("windows 7"))
                verbText = "Pin to Start Men&u";

            if (currentVersion == "6.3")
                verbText = "&Pin to Start";

            DoVerb(shortcut.Item, verbText);

        }

        /// <summary>
        /// Unpins shortcut from the start menu
        /// </summary>
        /// <example>
        /// <code>
        /// Shortcut shortcut = new Shortcut("path to shortcut");
        /// shortcut.UnpinFromStartMenu();
        /// </code>
        /// </example>
        /// <seealso cref="DoVerb"/>
        /// <param name="shortcut"></param>
        public static void UnpinFromStartMenu(this Shortcut shortcut)
        {
            string verbText = "";
            if (currentVersion.ToLower().Contains("windows 7"))
                verbText = "Unpin from Start Men&u";

            if (currentVersion == "6.3")
                verbText = "Un&pin from Start";

            DoVerb(shortcut.Item, verbText);
        }

        /// <summary>
        /// Internal function to execute Explorer Verbs
        /// </summary>
        /// <param name="shortcut"></param>
        /// <param name="verbText"></param>
        private static void DoVerb(Shortcut shortcut, string verbText)
        {
            foreach (dynamic verb in shortcut.Item.Verbs())
            {
                if (verb.Name == verbText)
                {
                    if (verb != null)
                        verb.DoIt();
                }
            }
        }
    }
}
