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
using System.Linq;
using Microsoft.Win32;
using Lib.ExtensionMethods;

namespace Lib
{
    /*
        The main ActiveSetup class
        Contains all methods for performing basic Active Setup actions
    */
    /// <summary>
    /// The main <c>ActiveSetup</c> class.
    /// Contains all methods for performing basic Active Setup actions.
    /// <list type="bullet">
    /// <item>
    /// <term>Set</term>
    /// <description>Adds program to Active Setup</description>
    /// </item>
    /// <item>
    /// <term>Undo</term>
    /// <description>Removes program from Active Setup</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>This class can set and undo active setup.</para>
    /// <para>These operations can be performed on both x64 and x86 operating systems</para>
    /// </remarks>
    public static class CMActiveSetup
    {
        // Set Active Setup registry keys into HKEY_LOCAL_MACHINE\SOFTWARE or HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node
        /// <summary>
        /// Set Active Setup registry keys into HKEY_LOCAL_MACHINE\SOFTWARE
        /// or HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node
        /// by providing <paramref name="ComponentID"/> followerd by <paramref name="StubPath"/>, <paramref name="Is32bitApp"/>, optionally <paramref name="Version"/> and <paramref name="Locale"/>
        /// </summary>
        /// <example>
        /// <code>
        /// ActiveSetup.Set("Google_Chrome_71", @"C:\ProgramData\Active Setup\Google_Chrome_71\ActiveSetup.cmd", false);
        /// </code>
        /// </example>
        /// <seealso cref="CMActiveSetup.Undo(string)"/>
        /// <seealso cref="CMWinRegistry.GetActiveSetupKeys()"/>
        /// <param name="ComponentID">The name of the program to Unset, wildcards ? or * are allowerd, non case sensitive</param>
        /// <param name="StubPath">The path where active setup script or batch is located</param>
        /// <param name="Is32bitApp">Specify if the application for which active setup should be create is 32 bit or 64</param>
        /// <param name="Version">Version of the Active Setup Key</param>
        /// <param name="Locale">Specify locale, if needed</param>
        public static void Set(string ComponentID, string StubPath, bool Is32bitApp, string Version = "1,0", string Locale = "*")
        {
            List<CMWinRegistry.WinRegPath> machineKeys = CMWinRegistry.GetActiveSetupKeys().Where(x => x.Scope == "Machine").ToList();
         
            if (Environment.Is64BitOperatingSystem && Is32bitApp)
                machineKeys = machineKeys.Where(x => x.RegPath.Contains("Wow6432Node")).ToList();
            else
                machineKeys = machineKeys.Where(x => !x.RegPath.Contains("Wow6432Node")).ToList();

            RegistryKey activeSetupKey = CMWinRegistry.RegKeyFromString(machineKeys[0].RegPath, true).CreateSubKey(ComponentID);
            activeSetupKey.SetValue(ComponentID, RegistryValueKind.String);
            activeSetupKey.SetValue(StubPath, RegistryValueKind.String);
            activeSetupKey.SetValue(Version, RegistryValueKind.String);
            activeSetupKey.SetValue(Locale, RegistryValueKind.String);
        }

        // Undo all related Active Setup registry keys
        /// <summary>
        /// Undo all related <paramref name="ComponentID"/> Active Setup registry keys.
        /// </summary>
        /// <example>
        /// <code>
        /// ActiveSetup.Undo("*chrome*");
        /// </code>
        /// </example>
        /// <seealso cref="CMActiveSetup.Set(string, string, bool, string, string)"/>
        /// <seealso cref="CMWinRegistry.GetActiveSetupKeys()"/>
        /// <param name="ComponentID">The name of the program to Unset, wildcards ? or * are allowerd, non case sensitive</param>
        public static void Undo(string ComponentID)
        {
            // Delete registry keys
            foreach (CMWinRegistry.WinRegPath activeSetupKey in CMWinRegistry.GetActiveSetupKeys())
            {
                foreach (string regKey in CMWinRegistry.GetSubKeyNames(activeSetupKey.RegPath))
                {
                    if (regKey.Like(ComponentID))
                    {
                        try
                        {
                            CMWinRegistry.RegKeyFromString(activeSetupKey.RegPath, true).DeleteSubKey(regKey);
                        }
                        catch (Exception ex)
                        {
                            //Support.WriteLog(ex.Message.ToString());
                        }
                    }
                }
            }
        }



        // Internal class to provide list of Active Setup registry keys
        /// <summary>
        /// Internal class to provide list of Active Setup registry keys.
        /// </summary>
        private class ActiveSetupKey
        {
            public string Scope { get; set; }
            public string RegPath { get; set; }
        }
    }
}
