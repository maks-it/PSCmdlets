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
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Lib
{
    /// <summary>
    /// 
    /// </summary>
    public static class CMNetwork
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostUri"></param>
        /// <param name="portNumber"></param>
        /// <returns></returns>
        public static bool PingHostPort(string hostUri, int portNumber)
        {
            try
            {
                using (var client = new TcpClient(hostUri, portNumber))
                    return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error pinging host:'" + hostUri + ":" + portNumber.ToString() + "'");
                return false;
            }
        }

        public static bool PingHostUDPPort(string hostUri, int portNumber)
        {
            // This constructor arbitrarily assigns the local port number.
            using (UdpClient udpClient = new UdpClient(portNumber)) {
                try
                {
                    udpClient.Connect(hostUri, portNumber);

                    // Sends a message to the host to which you have connected.
                    Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

                    udpClient.Send(sendBytes, sendBytes.Length);


                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    string returnData = Encoding.ASCII.GetString(receiveBytes);

                    // Uses the IPEndPoint object to determine which of these two hosts responded.
                    //Console.WriteLine("This is the message you received " + returnData.ToString());
                    //Console.WriteLine("This message was sent from " + RemoteIpEndPoint.Address.ToString() + " on their port number " + RemoteIpEndPoint.Port.ToString());

                    return true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(e.ToString());

                    return false;
                }

            }


        }
    }
}
