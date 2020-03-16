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

using System.Management.Automation;
using System.IO;

namespace PSCmdlets
{
    [Cmdlet(VerbsCommon.Get, "DrivesInfo")]
    [OutputType(typeof(GetDrivesInfo_Output))]
    public class GetDrivesFreeSpace : Cmdlet
    {
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                    WriteObject(new GetDrivesInfo_Output() {
                        DriveName = drive.Name,
                        UserFreeSpace = drive.AvailableFreeSpace,
                        FreeSpace = drive.TotalFreeSpace,
                        TotalSize = drive.TotalSize,
                        DriveType = drive.DriveType.ToString(),
                        DriveFormat = drive.DriveFormat
                    });
            }
        }
    }

    public class GetDrivesInfo_Output
    {
        public string DriveName { get; set; }
        public long UserFreeSpace { get; set; }
        public long FreeSpace { get; set; }
        public long TotalSize { get; set; }
        public string DriveType { get; set; }
        public string DriveFormat { get; set; }
    }
}