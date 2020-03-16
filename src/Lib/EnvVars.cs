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
using Microsoft.Win32;

namespace Lib
{
    /*
        Main EnvVars class
        Allows to Set and Unset environement variables
    */
    /// <summary>
    /// 
    /// </summary>
    public static class CMEnvVars
    {
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPath"></param>
        public static void AddToPath(string newPath)
        {
            string pathRegKey = @"System\CurrentControlSet\Control\Session Manager\Environment";

            string pathEnvVar = Registry.LocalMachine.OpenSubKey(pathRegKey).GetValue("PATH").ToString();

            bool found = false;
            foreach (string path in pathEnvVar.Split(';'))
            {
                if (path == newPath)
                    found = true;
            }

            if (found)
                pathEnvVar = pathEnvVar.Trim(';') + ";" + newPath;

            Registry.LocalMachine.OpenSubKey(pathRegKey).SetValue("PATH", pathEnvVar);
        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="envName"></param>
        /// <param name="envValue"></param>
        /// <param name="envTarghet"></param>
        public static void Set(string envName, string envValue, string envTarghet = "")
        {
            envTarghet = envTarghet.ToLower();

            EnvironmentVariableTarget targhet = new EnvironmentVariableTarget();
            switch (envTarghet)
            {
                case "machine":
                    {
                        targhet = EnvironmentVariableTarget.Machine;
                        break;
                    }
                case "user":
                    {
                        targhet = EnvironmentVariableTarget.User;
                        break;
                    }
                case "process":
                    {
                        targhet = EnvironmentVariableTarget.Process;
                        break;
                    }
                default:
                    {
                        targhet = EnvironmentVariableTarget.Machine;
                        break;
                    }
            }

            // Determine whether the environment variable exists.
            if (Environment.GetEnvironmentVariable(envName) == null)
            {
                // If it doesn't exist, create it.
                Environment.SetEnvironmentVariable(envName, envValue, targhet);
            }
        }

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="envName"></param>
        /// <param name="envTarghet"></param>
        public static void UnSet(string envName, string envTarghet = "")
        {
            EnvironmentVariableTarget targhet = new EnvironmentVariableTarget();
            switch (envTarghet)
            {
                case "Machine":
                    {
                        targhet = EnvironmentVariableTarget.Machine;
                        break;
                    }
                case "User":
                    {
                        targhet = EnvironmentVariableTarget.User;
                        break;
                    }
                case "Process":
                    {
                        targhet = EnvironmentVariableTarget.Process;
                        break;
                    }
                default:
                    {
                        targhet = EnvironmentVariableTarget.Machine;
                        break;
                    }
            }

            // Determine whether the environment variable exists.
            if (Environment.GetEnvironmentVariable(envName, targhet) != null)
                Environment.SetEnvironmentVariable(envName, null, targhet);
        }
    }
}
