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
using System.Data;
using System.Data.SqlClient;

namespace Lib
{
    // Main CMSQLClient class
    /// <summary>
    /// Main <c>CMSQLClient</c> class.
    /// </summary>
    public static class CMSQLClient
    {

        //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connString, string query)
        {
            int rows = 0;
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();

                foreach (string batch in query.Split(new string[] { "GO\r\n" }, StringSplitOptions.None))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(batch, sqlConnection))
                    {
                        sqlCommand.CommandTimeout = 0;
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            rows = sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

            return rows;
        }

        // Execute SQL Server query and returns result as DataTable
        /// <summary>
        /// Execute SQL Server query and returns result as DataTable
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataTable ExecuteQuery(string connString, string query) {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connString))
            {
                sqlConnection.Open();
                foreach (string batch in query.Split(new string[] { "GO\r\n" }, StringSplitOptions.None))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(batch, sqlConnection))
                    {
                        sqlCommand.CommandTimeout = 0;
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            dataTable.Load(sqlCommand.ExecuteReader());
                        }
                    }
                }
            }

            return dataTable;
        }
    }
}
