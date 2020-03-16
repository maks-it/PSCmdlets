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
using System.Management.Automation;
using Lib;

namespace PSCmdlets
{
    [Cmdlet(VerbsCommon.New, "RegValue")]
    [OutputType(typeof(string))]
    public class NewRegValue : Cmdlet
    {

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string RegPath { get; set; }

        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string ValueName { get; set; }

        [Parameter(Position = 3, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string ValueType { get; set; } 

        [Parameter(Position = 4, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public object ValueData { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            try
            {
                CMWinRegistry.NewRegValue(RegPath, ValueName, ValueType, ValueData);
                WriteObject("OK: Registry key " + RegPath + " created and value " + ValueName + " = " + ValueData.ToString() + " set.");
            }
            catch (Exception ex)
            {
                WriteObject("ERR: " + ex.Message.ToString());
            }

        }
    }
}
