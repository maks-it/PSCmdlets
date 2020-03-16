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
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;
using Lib;
using Lib.ExtensionMethods;

namespace PSCmdlets
{
    [Cmdlet(VerbsCommon.Remove, "UninstallKeysByVal")]
    [OutputType(typeof(string))]
    public class RemoveRegkeyByVal : Cmdlet
    {
        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Value { get; set; } = string.Empty;



        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            try
            {
                List<dynamic> keysToDelete = new List<dynamic>();
                foreach (CMWinRegistry.WinRegPath uninstallPath in CMWinRegistry.GetUninstallKeys())
                {
                    foreach (string appKeyName in CMWinRegistry.RegKeyFromString(uninstallPath.RegPath).GetSubKeyNames())
                    {
                        foreach (CMWinRegistry.RegValue regValue in CMWinRegistry.GetRegValues(CMWinRegistry.RegKeyFromString(uninstallPath.RegPath + "\\" + appKeyName)))
                        {
                            if (regValue.Data.ToString().Like(Value))
                            {
                                keysToDelete.Add(new
                                {
                                    RootKey = uninstallPath.RegPath,
                                    ParentKey = appKeyName
                                });
                            }
                        }
                    }
                }

                keysToDelete = keysToDelete.Distinct().ToList();

                foreach (dynamic regKey in keysToDelete)
                {
                    CMWinRegistry.RegKeyFromString((string)regKey.RootKey, true).DeleteSubKeyTree((string)regKey.ParentKey);
                    WriteObject("OK: Deleted " + (string)regKey.ParentKey + " in " + (string)regKey.RootKey + "...");
                }


            }
            catch (Exception ex)
            {
                WriteObject("ERR: " + ex.Message.ToString());
            }

        }
    }
}