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
using System.Diagnostics;
using Lib.ExtensionMethods;

namespace Lib
{
    /*
        Main CustomProcess class
        Provide helper methods to Start and Kill processe
    */
    /// <summary>
    /// Main <c>CustomProcess</c> class.
    /// Provide helper methods to Start and Kill processe.
    /// <list type="bullet">
    /// <item>
    /// <term>Start</term>
    /// <description>Starts new process</description>
    /// </item>
    /// <item>
    /// <term>Kill</term>
    /// <description>Kills processes by name</description>
    /// </item>
    /// </list>
    /// </summary>
    public static class CMProcess
    {
        // Run process silently
        /// <summary>
        /// Starts new process
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Arguments"></param>
        /// <param name="Timeout"></param>
        /// <param name="Silent"></param>
        public static void Start(string FileName, string Arguments, int Timeout, bool Silent)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo(FileName);
                if (Arguments != string.Empty)
                    processInfo.Arguments = Arguments;


                if (Silent)
                {
                    processInfo.UseShellExecute = false;
                    processInfo.CreateNoWindow = true;
                }

                Process proc = new Process();
                proc.StartInfo = processInfo;
                proc.Start();
                proc.WaitForExit(Timeout * 1000);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Kill process by name
        /// <summary>
        /// Kill process by name.
        /// </summary>
        /// <param name="process">Process name. Accept wildcards '*' or '?')</param>
        public static void Kill(string process)
        {
            foreach (Process proc in Process.GetProcesses())
            {
                try
                {
                    if (proc.ProcessName.Like(process))
                        proc.Kill();
                }
                catch { }
            }
        }
    }
}
