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
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace Lib
{
    /*
        Main Common class
    */
    /// <summary>
    /// Main <c>LibInfo</c> class.
    /// </summary>
    public static class Support
    {
        // Returns common library version
        /// <summary>
        /// Returns common library version.
        /// </summary>
        /// <returns></returns>
        public static string Version()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
        }

        // Write debug log
        /// <summary>
        /// Write debug log.
        /// </summary>
        /// <param name="message"></param>
        public static void _WriteLog(object message)
        {
            //C:\Windows\System32\config\systemprofile\AppData
            //C:\Windows\SysWOW64\config\systemprofile\AppData

            string path = AppDomain.CurrentDomain.BaseDirectory + Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".log";


            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                if (new FileInfo(path).Length / 1048576 > 20)
                    File.Delete(path);
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(message);
            }
        }
    }
}
