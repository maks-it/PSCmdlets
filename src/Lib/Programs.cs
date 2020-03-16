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
using System.Data;
using System.Linq;
using System.Text;


namespace Lib
{
    /*
     */
    /// <summary>
    /// 
    /// </summary>
    public static class CMPrograms
    {
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable Get()
        {
            List<InstalledItem> installedItems = new List<InstalledItem>();
            foreach (CMWinRegistry.WinRegPath uninstallPath in CMWinRegistry.GetUninstallKeys())
            {
                foreach (string appKeyName in CMWinRegistry.RegKeyFromString(uninstallPath.RegPath).GetSubKeyNames())
                {
                    InstalledItem installedItem = new InstalledItem() { Scope = uninstallPath.Scope, RegValues = CMWinRegistry.GetRegValues(CMWinRegistry.RegKeyFromString(uninstallPath.RegPath + "\\" + appKeyName)) };
                    installedItems.Add(installedItem);
                }
            }

            List<string> columnNames = new List<string>();
            foreach (InstalledItem installedItem in installedItems)
            {
                foreach (CMWinRegistry.RegValue regValue in installedItem.RegValues)
                {
                    if (regValue.Name != "")
                    {
                        columnNames.Add(regValue.Name);
                    }
                    else
                    {
                        columnNames.Add("WindowsKB");
                    }
                }
            }

            columnNames = columnNames.Distinct().ToList();

            DataTable result = new DataTable("InstalledPrograms");
            result.Columns.Add("Scope", typeof(String));
            foreach (string columnName in columnNames)
            {
                result.Columns.Add(columnName, typeof(String));
            }

            foreach (InstalledItem installedItem in installedItems)
            {
                DataRow row = result.NewRow();
                row["Scope"] = installedItem.Scope;
                foreach (CMWinRegistry.RegValue regValue in installedItem.RegValues)
                {
                    if (regValue.Name != "")
                    {
                        row[regValue.Name] = regValue.Data;
                    }
                    else
                    {
                        row["WindowsKB"] = regValue.Data;
                    }
                }
                result.Rows.Add(row);
            }

            return result;
        }



        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DisplayName"></param>
        /// <param name="DisplayVersion"></param>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        public static List<string> Uninstall(string DisplayName, string DisplayVersion = "", string Arguments = "")
        {
            List<string> response = new List<string>();

            foreach (DataRow row in Get().Rows)
            {
                if (row["Scope"].ToString() == "Machine" || row["Scope"].ToString() == Environment.UserName)
                {
                    if (row["DisplayName"].ToString().ToLower().Contains(DisplayName.ToLower()) && row["DisplayVersion"].ToString().ToLower().Contains(DisplayVersion.ToLower()))
                    {
                        UninstallString uninstallString = new UninstallString(row["UninstallString"].ToString());
                        CMProcess.Start(uninstallString.FileName, (uninstallString.Arguments + " " + Arguments).Trim(), 60, true);

                        response.Add(uninstallString.FileName + " " + (uninstallString.Arguments + " " + Arguments).Trim());
                    }
                }
            }

            return response;
        }

       

        private class InstalledItem
        {
            public string Scope { get; set; }
            public List<CMWinRegistry.RegValue> RegValues { get; set; }
        }

        private class UninstallString
        {
            public string FileName { get; set; }
            public string Arguments { get; set; }

            public UninstallString(string rawString)
            {
                if (rawString.ToLower().Contains("msiexec"))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (char c in rawString)
                    {
                        if (c != '/')
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            break;
                        }
                    }

                    FileName = sb.ToString().Trim();
                    Arguments = rawString.Replace(FileName, "").Trim();

                    if (Arguments.Substring(0, 2).ToLower() == "/i")
                    {
                        Arguments = "/X" + Arguments.Substring(2, Arguments.Length - 2);
                    }
                }

                if (!rawString.ToLower().Contains("msiexec") && rawString.ToLower().Contains(".exe"))
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (char c in rawString)
                    {
                        if (!sb.ToString().Contains(".exe"))
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            break;
                        }
                    }

                    FileName = sb.ToString().Trim();
                    if (FileName.Substring(0, 1) == "\"")
                        FileName += "\"";
                    Arguments = rawString.Replace(FileName, "").Trim();
                }


            }
        }
    }

}