using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using Android.Net;
using System.Net.NetworkInformation;

namespace Gate
{
    static class TCP
    {
        static TcpClient tcpClient = null;
        static NetworkStream tcpStream = null;
        public static string ip = null;

        static private void GetStream()
        {
            tcpStream = tcpClient.GetStream();
            tcpStream.ReadTimeout = 5000;
            tcpStream.WriteTimeout = 2000;
        }

        static public bool isPhoneOnline(Activity a)
        {
            var cm = (ConnectivityManager)a.GetSystemService(Context.ConnectivityService);
            NetworkInfo netInfo = cm.ActiveNetworkInfo;
            if (netInfo != null && netInfo.IsConnectedOrConnecting)
            {
                return true;
            }
            return false;
        }
       
        static public bool ConnectWithDialog(string ip, Activity a)
        {
            bool ok = false;
            if (isPhoneOnline(a) & isConnectable(ip))
            {
                try
                {
                    tcpClient = new TcpClient(ip, 1025);
                    ok = true;
                    TCP.ip = ip;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (ok)
                    GetStream();
            }
            else
            {
                a.RunOnUiThread(() =>
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(a);
                    AlertDialog alertDialog = builder.Create();
                    alertDialog.SetTitle("No Connection");
                    alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    alertDialog.SetMessage("No connection to reader!");
                    alertDialog.SetButton("OK", (s, ev) =>
                    {
                    });
                    alertDialog.Show();
                });
            }
            return ok;
        }

        static public bool isConnectable(string ip)
        {
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(ip, 500);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        static public bool isTCPNull()
        {
            return tcpClient == null;
        }

        static public void Close()
        {
            if (tcpStream != null)
            {
                tcpClient.Client.Disconnect(false);
                tcpStream.Close();
                tcpStream = null;
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }
        }

        static public bool WriteOnly(string cmd)
        {
            // Send the message to the connected TcpServer. 
            Byte[] d = System.Text.Encoding.ASCII.GetBytes(cmd);
            Byte[] d2 = new byte[cmd.Length + 2];
            d.CopyTo(d2, 1);
            d2[0] = 0x2;
            d2[cmd.Length + 1] = 0x3;
            bool Result = true;
            try
            {
                tcpStream.Write(d2, 0, d2.Length);
            }
            catch
            {
                Result = false;
            }
            return Result;
        }


        /// <summary>
        /// send a command to the readerr
        /// </summary>
        /// <param name="cmd">Command to send</param>
        /// <param name="data">result if returns true</param>
        /// <returns>true if all is well</returns>
        static public bool Write(string cmd, out string data)
        {
            data = "";
            if (!WriteOnly(cmd))
                return false;

            // Read the first batch of the TcpServer response bytes.
            bool stxFlag = false;
            bool ok = false;
            byte[] bdata = new byte[0];
            while (true)
            {
                int b = -1;
                try
                {
                    b = tcpStream.ReadByte();
                }
                catch { }
                if (b == -1)
                    break;
                if (b == 0x2)
                    stxFlag = true;
                else
                {
                    if (stxFlag)
                    {
                        if (b == 0x3)
                        {
                            ok = true;
                            break;
                        }
                        Array.Resize(ref bdata, bdata.Length + 1);
                        bdata[bdata.Length - 1] = (byte)b;
                    }
                }
            }
            if (ok)
                data = System.Text.Encoding.ASCII.GetString(bdata);
            Console.WriteLine(ok);
            return ok;
        }
    }
}