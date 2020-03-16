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
using System.Collections.Generic;
using Microsoft.Win32;
using Lib.ExtensionMethods;

namespace Lib
{
    /*
        
    */
    /// <summary>
    /// 
    /// </summary>
    public static class CMWinRegistry
    {
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="valueName"></param>
        /// <param name="valueType"></param>
        /// <param name="valueData"></param>
        /// <returns></returns>
        public static bool NewRegValue(string path, string valueName, string valueType, object valueData)
        {
            bool response = false;

            if (NewRegKey(path))
            {
                try
                {
                    RegistryKey regKey = RegKeyFromString(path, true);
                    regKey.SetValue(valueName, valueData, RegValueKindFromString(valueType));
                    response = true;
                }
                catch (Exception ex)
                {
                    
                }
            }

            return response;
            
        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool NewRegKey(string path)
        {
            string[] partialPaths = path.Split('\\');

            string validPath = partialPaths[0] + '\\';
            for (int i = 1; i < partialPaths.Length; i++)
            {
                if (RegKeyFromString(validPath + '\\' + partialPaths[i]) == null)
                {
                    RegistryKey regKey = RegKeyFromString(validPath, true);
                    regKey.CreateSubKey(partialPaths[i]);
                }

                validPath += '\\' + partialPaths[i];
            }

            RegistryKey newKey = RegKeyFromString(path, true);
            if (newKey != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<RegValue> GetRegValues(RegistryKey key)
        {
            List<RegValue> response = new List<RegValue>();

            foreach (var keyValueName in key.GetValueNames())
            {
                RegValue regValue = new RegValue() { Path = key.Name, Name = keyValueName, Data = key.GetValue(keyValueName), Type = key.GetValueKind(keyValueName).ToString() };
                response.Add(regValue);
            }

            return response;
        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<RegUser> GetUsersFromHiveList()
        {
            List<RegUser> response = new List<RegUser>();

            foreach (RegValue regValue in GetRegValues(RegKeyFromString(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\hivelist")))
            {
                string xName = regValue.Name;
                string xValue = regValue.Data.ToString();


                if (xName.Like("*USER*") && !xName.Like("*Classes*") && xValue.Like("*Users*"))
                {
                    string userSID = xName.Split('\\')[3];
			        string userName = xValue.Split('\\')[4];

                    RegUser regUser = new RegUser() { Name = userName, SID = userSID };
                    response.Add(regUser);
                }
            }

            return response;
        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="writable"></param>
        /// <returns></returns>
        public static RegistryKey RegKeyFromString(string path, bool writable = false)
        {
            RegistryKey response = null;

            List<RegistryKey> regRoots = new List<RegistryKey>() {
                Registry.ClassesRoot,
                Registry.CurrentUser,
                Registry.LocalMachine,
                Registry.Users,
                Registry.CurrentUser
            };

            foreach (RegistryKey regRoot in regRoots)
            {
                if (path.ToLower().Contains(regRoot.Name.ToLower()))
                {
                    string cleanPath = path.Substring(regRoot.Name.Length, path.Length - regRoot.Name.Length).Trim('\\');
                    response = regRoot.OpenSubKey(cleanPath, writable);

                    break;
                }
            }

            return response;
        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static RegistryValueKind RegValueKindFromString(string valueType)
        {
            RegistryValueKind valueKind = RegistryValueKind.Unknown;
            switch (valueType.ToLower())
            {
                case "binary":
                    {
                        valueKind = RegistryValueKind.Binary;
                        break;
                    }

                case "dword":
                    {
                        valueKind = RegistryValueKind.DWord;
                        break;
                    }

                case "expandedstring":
                    {
                        valueKind = RegistryValueKind.ExpandString;
                        break;
                    }
                case "multistring":
                    {
                        valueKind = RegistryValueKind.MultiString;
                        break;
                    }

                case "none":
                    {
                        valueKind = RegistryValueKind.None;
                        break;
                    }

                case "qword":
                    {
                        valueKind = RegistryValueKind.QWord;
                        break;
                    }

                case "string":
                    {
                        valueKind = RegistryValueKind.String;
                        break;
                    }

            }

            return valueKind;

        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regPath"></param>
        /// <returns></returns>
        public static string [] GetSubKeyNames(string regPath) {
            return RegKeyFromString(regPath).GetSubKeyNames();
        }

        // Return all uninstall registry keys available into machine
        /// <summary>
        /// Return all uninstall registry keys available into machine.
        /// </summary>
        /// <returns></returns>
        public static List<WinRegPath> GetUninstallKeys()
        {
            List<WinRegPath> uninstallKeys = new List<WinRegPath>();

            //Machine Level Uninstall Keys
            uninstallKeys.Add(new WinRegPath() { Scope = "Machine", RegPath = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Uninstall" });
            if (Environment.Is64BitOperatingSystem)
            {
                uninstallKeys.Add(new WinRegPath() { Scope = "Machine", RegPath = @"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall" });
            }

            //Users Level Uninstall Keys
            foreach (RegUser regUser in GetUsersFromHiveList())
            {
                try
                {
                    string regPath = @"HKEY_USERS\" + regUser.SID + @"\Software\Microsoft\Windows\CurrentVersion\Uninstall";
                    if (RegKeyFromString(regPath) != null)
                    {
                        uninstallKeys.Add(new WinRegPath() { Scope = regUser.Name, RegPath = regPath });
                    }
                }
                catch
                {

                }

            }

            return uninstallKeys;
        }

        // Public method which returns all Active Setup registry paths into HKLM and from HKCU for each user (depends from thread access rights)
        /// <summary>
        /// <para>Allow to get ActiveSetup registry paths</para>
        /// </summary>
        /// <returns>Returns List of custom class ActiveSetupKey</returns>
        /// <example>
        /// <code>
        /// GetActiveSetupKeys();
        /// </code>
        /// </example>
        /// <seealso cref="CMActiveSetup.Set(string, string, bool, string, string)"/>
        /// <seealso cref="CMActiveSetup.Undo(string)"/>
        public static List<WinRegPath> GetActiveSetupKeys()
        {
            List<WinRegPath> activeSetupKeys = new List<WinRegPath>();

            activeSetupKeys.Add(new WinRegPath() { Scope = "Machine", RegPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Active Setup\Installed Components" });
            if (Environment.Is64BitOperatingSystem)
                activeSetupKeys.Add(new WinRegPath() { Scope = "Machine", RegPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Active Setup\Installed Components" });

            //Users Level Uninstall Keys
            foreach (CMWinRegistry.RegUser regUser in CMWinRegistry.GetUsersFromHiveList())
            {
                try
                {
                    if (CMWinRegistry.RegKeyFromString(@"HKEY_USERS\" + regUser.SID + @"\Software\Microsoft\Active Setup\Installed Components") != null)
                        activeSetupKeys.Add(new WinRegPath() { Scope = regUser.Name, RegPath = @"HKEY_USERS\" + regUser.SID + @"\Software\Microsoft\Active Setup\Installed Components" });
                }
                catch (Exception ex)
                {
                    //Support.WriteLog(ex.Message.ToString());
                }
            }

            return activeSetupKeys;
        }

        //
        /// <summary>
        /// 
        /// </summary>
        public class WinRegPath
        {
            public string Scope { get; set; }
            public string RegPath { get; set; }
        }

        //
        /// <summary>
        /// 
        /// </summary>
        public class RegValue
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public object Data { get; set; }
            public string Type { get; set; }
        }

        //
        /// <summary>
        /// 
        /// </summary>
        public class RegUser
        {
            public string Name { get; set; }
            public string SID { get; set; }
        }
    }
}
