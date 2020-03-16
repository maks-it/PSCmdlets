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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lib
{
    /*
        Main Parse class
        Used to parse and convert strings
    */
    /// <summary>
    /// Main <c>Parse</c> class.
    /// Used to parse and convert strings.
    /// <list type="bullet">
    /// <item>
    /// <term>ConvertFromJson</term>
    /// <description>Convert JSON formatted string to Dynamic object</description>
    /// </item>
    /// </list>
    /// </summary>
    public static class Parse
    {
        // Convert stroing from JSON to Dynamic object
        /// <summary>
        /// Convert stroing from JSON to Dynamic object.
        /// </summary>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static dynamic ConvertFromJson(string JsonString)
        {
            //allow working with JSON without need to know its structure
            dynamic result = JObject.Parse(JsonString);
            return result;
        }


        //Convert Dynamic object to JSON string
        /// <summary>
        /// Convert Dynamic object to JSON string.
        /// </summary>
        /// <param name="JsonObject"></param>
        /// <returns></returns>
        public static string ConvertToJson(dynamic JsonObject)
        {
            //
            return JsonConvert.SerializeObject(JsonObject);
        }
    }
}
