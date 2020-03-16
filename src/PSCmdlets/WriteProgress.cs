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
using System.Threading.Tasks;
using System.Management.Automation;
using System.Data;

namespace PSCmdlets
{
    [Cmdlet(VerbsCommunications.Write, "Progress")]
    public class WriteProgress : Cmdlet
    {
        [Parameter(Position = 1, Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public object Text { get; set; } = string.Empty;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            
        }

     

        public static void ProgressBar(int progress, int total)
        {

            string totalsText = progress.ToString() + " of " + total.ToString();

            //get console width
            int consoleWidth = Console.WindowWidth;

            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = consoleWidth - 2 - totalsText.Length;
            Console.Write("] "); //end
            Console.Write(totalsText);

            Console.SetCursorPosition(0, Console.CursorTop - 1);

            float onechunk = ((float)consoleWidth - 2 - totalsText.Length) / total;

            //draw filled part
            int position = 1;
            for (int i = 1; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            /*draw unfilled part
            for (int i = position; i <= consoleWidth - 2; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }*/



            //back to defaults
            Console.BackgroundColor = ConsoleColor.Black;

            if (progress == total)
            {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
            }
            else
            {

            }
        }
    }
}