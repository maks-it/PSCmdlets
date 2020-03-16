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

using System.Threading;
using System.Globalization;

namespace Lib
{
    /*
        The main Cultures class
        Contains all methods for performing basic Cultures management
    */
    /// <summary>
    /// The main <c>Cultures</c> class.
    /// Contains all methods for performing basic Cultures management.
    /// </summary>
    public static class CMCulture
    {
        // Set culture for current thread
        /// <summary>
        /// Set culture <paramref name="culture"/> for current thread
        /// </summary>
        /// <param name="culture"></param>
        public static void Set(string culture = "")
        {
            CultureInfo threadCulture = CultureInfo.InvariantCulture;

            if (culture != "")
                threadCulture = CultureInfo.CreateSpecificCulture(culture);

            Thread.CurrentThread.CurrentUICulture = threadCulture;
            Thread.CurrentThread.CurrentCulture = threadCulture;            
        }
    }
}
