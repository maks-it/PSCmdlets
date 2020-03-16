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
using System.IO;
using System.Globalization;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace Lib
{
    // Main CMSchedule class
    /// <summary>
    /// Main <c>CMSchedule</c> class.
    /// This calss allows you to 
    /// </summary>
    public static class CMSchedule
    {
        #region Public metods to work with schedulings
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="RunMode"></param>
        /// <param name="RunDay"></param>
        /// <param name="RunTime">String in "HH:mm:ss" format</param>
        /// <param name="CurrentDateTimeUtc">Mandatory ISO 8601 UTC DateTime string</param>
        /// <param name="ExecHistoryPath"></param>
        /// <returns></returns>
        public static bool Confirm(string Name, string RunMode, string RunDay, string RunTime, string CurrentDateTimeUtc, string ExecHistoryPath = "")
        {
            DateTime currentDateTimeUtc = DateTime.Parse(CurrentDateTimeUtc, null, DateTimeStyles.RoundtripKind);
            TimeSpan runTime = TimeSpan.ParseExact(RunTime, "g", CultureInfo.InvariantCulture);

            //search last execution by name into execution history
            DateTime foundLastExecution = DateTime.MinValue;
            foreach (LastExecution lastExecution in GetExecHistory(ExecHistoryPath).ExecHistory)
            {
                if (lastExecution.Name == Name)
                {
                    foundLastExecution = lastExecution.Date;
                    break;
                }
            }

            bool response = false;
            switch (RunMode)
            {
                case "Hourly":
                    {
                        break;
                    }
                case "Daily":
                    {
                        response = Daily(foundLastExecution, runTime, currentDateTimeUtc);
                        break;
                    }
                case "WeekDay":
                    {
                        response = Weekday(foundLastExecution, RunDay, runTime, currentDateTimeUtc);
                        break;
                    }
                case "Monthly":
                    {
                        break;
                    }
                default:
                    {
                        response = false;
                        break;
                    }
            }

            return response;
        }

        // Saves into exechistory.json last execution time by name
        /// <summary>
        /// Saves into exechistory.json last execution time by name.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="CompletedDateTimeUtc"></param>
        /// <param name="Status"></param>
        /// <param name="ExecHistoryPath"></param>
        public static void UpdateExecHistory(string Name, string CompletedDateTimeUtc = "", string Status = "", string ExecHistoryPath = "")
        {
            ExecutionHistosy execHistory = GetExecHistory(ExecHistoryPath);

            DateTime dateTimeUtc = DateTime.UtcNow;
            if (CompletedDateTimeUtc != string.Empty)
            {
                dateTimeUtc = DateTime.Parse(CompletedDateTimeUtc, null, DateTimeStyles.RoundtripKind);
            }

            bool isUpdated = false;
            for (int i = 0; i < execHistory.ExecHistory.Count; i++)
            {
                if (execHistory.ExecHistory[i].Name == Name)
                {
                    execHistory.ExecHistory[i].Date = dateTimeUtc;
                    execHistory.ExecHistory[i].Status = Status;
                    isUpdated = true;
                }
            }



            if (!isUpdated)
                execHistory.ExecHistory.Add(new LastExecution() { Name = Name, Date = dateTimeUtc, Status = Status });

            string jsonString = JsonConvert.SerializeObject(execHistory, Formatting.Indented);

            File.WriteAllText(ExecHistoryPath, jsonString);
        }
        #endregion

        #region Execution evaluation methods
        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FoundLastExecution"></param>
        /// <param name="RunTime"></param>
        /// <param name="CurrentDateTime"></param>
        /// <returns></returns>
        private static bool Hourly(DateTime FoundLastExecution, TimeSpan RunTime, DateTime CurrentDateTime)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FoundLastExecution"></param>
        /// <param name="RunTime"></param>
        /// <param name="CurrentDateTime"></param>
        /// <returns></returns>
        private static bool Daily (DateTime FoundLastExecution, TimeSpan RunTime , DateTime CurrentDateTime)
        {
            bool response = false;

            if (FoundLastExecution.DayOfYear < CurrentDateTime.DayOfYear || FoundLastExecution.Year < CurrentDateTime.Year)
            {
                Debug.WriteLine("Job hasn't been executed today.");
                if (CurrentDateTime.TimeOfDay >= RunTime)
                {
                    Debug.WriteLine("RunTime reached.");
                    response = true;
                }
                else
                    Debug.WriteLine("RunTime not reached.");
            }
            else
                Debug.WriteLine("Job has been executed today.");

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FoundLastExecution"></param>
        /// <param name="RunDay"></param>
        /// <param name="RunTime"></param>
        /// <param name="CurrentDateTim"></param>
        /// <returns></returns>
        private static bool Weekday(DateTime FoundLastExecution, string RunDay, TimeSpan RunTime, DateTime CurrentDateTim)
        {
            bool response = false;

            if (CurrentDateTim.DayOfWeek.ToString() == RunDay)
                response = Daily(FoundLastExecution, RunTime, CurrentDateTim);
            else
                Debug.WriteLine("Current day is " + CurrentDateTim.DayOfWeek + " doesn't correspondinds to " + RunDay);

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool Monthly ()
        {
            return false;
        }
        #endregion

        #region Basic components to manage JSON
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExecHistoryPath"></param>
        /// <returns></returns>
        private static ExecutionHistosy GetExecHistory(string ExecHistoryPath = "")
        {
            // Assign default filename and path
            if (ExecHistoryPath == string.Empty)
                ExecHistoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"exechistory.json");

            // create empty json file if doesn't exists
            if (!File.Exists(ExecHistoryPath))
            {
                string jsonString = JsonConvert.SerializeObject(new ExecutionHistosy() { ExecHistory = new List<LastExecution>() }, Formatting.Indented);
                File.WriteAllText(ExecHistoryPath, jsonString);
            }

            // read all file text, convert to JSON and return object
            string jsonText = File.ReadAllText(ExecHistoryPath);
            return JsonConvert.DeserializeObject<ExecutionHistosy>(jsonText);
        }

        #region JSON structure classes
        //
        /// <summary>
        /// 
        /// </summary>
        public class LastExecution
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Status { get; set; }
        }

        //
        /// <summary>
        /// 
        /// </summary>
        public class ExecutionHistosy
        {
            /// <summary>
            /// 
            /// </summary>
            public IList<LastExecution> ExecHistory { get; set; }
        }
        #endregion

        #endregion
    }


}
