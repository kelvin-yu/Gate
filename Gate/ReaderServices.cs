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
using System.Threading;

using Gate.WebReference;
using Android.Preferences;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Android.Net.Wifi;


namespace Gate
{
    static class ReaderServices
    {
        static string data = "";

        public static void sendAccessLevel(List<AccessLevel> list, Activity a)
        {
            deleteAllAccessLevels(a);
            foreach (AccessLevel level in list)
            {
                string acc = "AA";
                acc += "\t" + level.name;
                foreach (DateTime startTime in level.weekTimeStart) //week start
                {
                    string[] time = startTime.ToString("HH:mm").Split(new char[] { ':' });
                    int hours = Convert.ToInt16(time[0]);
                    int minutes = Convert.ToInt16(time[1]);
                    int totalMinutes = hours * 60 + minutes;

                    acc += "\t" + Convert.ToString(totalMinutes);
                }
                foreach (DateTime endTime in level.weekTimeEnd) //week end
                {
                    string[] time = endTime.ToString("HH:mm").Split(new char[] { ':' });
                    int hours = Convert.ToInt16(time[0]);
                    int minutes = Convert.ToInt16(time[1]);
                    int totalMinutes = hours * 60 + minutes;

                    acc += "\t" + Convert.ToString(totalMinutes);
                }
                acc += "\t";
                foreach (string r1Access in level.weekReader1Access) //reader 1 access
                {
                    acc += r1Access;
                }
                acc += "\t";
                foreach (string r2Access in level.weekReader2Access) //reader 2 access
                {
                    acc += r2Access;
                }
                acc += level.usePassBack ? "\tY" : "\tN"; //passback
                acc += level.useDateRange ? "\tY" : "\tN"; //date range


                if (level.useDateRange)
                {
                    int start = 0, end = 0;

                    string[] values = level.dateStart.ToString("M/d/yyyy").Split(new char[] { '/' });
                    int year = Convert.ToInt16(values[2]);
                    int day = Convert.ToInt16(values[1]);
                    int month = Convert.ToInt16(values[0]);
                    TimeSpan startSpan = TimeSpan.FromTicks(new DateTime(year, month, day).Ticks).Subtract(TimeSpan.FromTicks(new DateTime(1980, 1, 1).Ticks));
                    start = (int)startSpan.Days;
                    values = level.dateEnd.ToString("M/d/yyyy").Split(new char[] { '/' });
                    year = Convert.ToInt16(values[2]);
                    day = Convert.ToInt16(values[1]);
                    month = Convert.ToInt16(values[0]);
                    TimeSpan endSpan = TimeSpan.FromTicks(new DateTime(year, month, day).Ticks).Subtract(TimeSpan.FromTicks(new DateTime(1980, 1, 1).Ticks));
                    end = (int)endSpan.Days;

                    acc += "\t" + start;
                    acc += "\t" + end;
                }

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(a);
                TCP tcp = new TCP();
                tcp.Connect(prefs.GetString("reader_ip", "192.168.2.180"), a);
                tcp.Write(acc, out data);
                tcp.Close();
            }
        }

        public static void sendCard(List<Card> list, Activity a)
        {
            deleteAllCards(a);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(a);
            TCP tcp = new TCP();
            tcp.Connect(prefs.GetString("reader_ip", "192.168.2.180"), a);
            foreach (Card card in list)
            {
                string c = "AC";
                c += "\t" + card.accessLevel;
                c += "\t" + card.cardCode;
                tcp.Write(c, out data);
            }
            tcp.Close();
        }

        public static void deleteAllAccessLevels(Activity a)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(a);
            TCP tcp = new TCP();
            tcp.Connect(prefs.GetString("reader_ip", "192.168.2.180"), a);
            tcp.Write("DA", out data);
            tcp.Close();
        }

        public static void deleteAllCards(Activity a)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(a);
            TCP tcp = new TCP();
            tcp.Connect(prefs.GetString("reader_ip", "192.168.2.180"), a);
            tcp.Write("DC", out data);
            tcp.Close();
        }

        public static void deleteAll(Activity a)
        {
            deleteAllAccessLevels(a);
            deleteAllCards(a);
        }

        public static void OpenGate(string readerNum, Activity a)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(a);
            TCP tcp = new TCP();
            tcp.Connect(prefs.GetString("reader_ip", "192.168.2.180"), a);
            tcp.Write("O" + readerNum, out data);
            tcp.Close();
        }

        public static void CreateOkDialog(string title, string message, Activity a)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(a);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (s, ev) =>
            {
            });
            alertDialog.Show();
        }

        static ProgressDialog bar;

        private static void CreateProgress(Activity a)
        {
            bar = new ProgressDialog(a);
            bar.SetCancelable(false);
            bar.SetMessage("Refreshing Information");
            bar.SetProgressStyle(ProgressDialogStyle.Horizontal);
            bar.Progress = 0;
            bar.Max = 100;
            bar.Show();
        }

        public static void UpdateInfo(Activity a)
        {
            Console.WriteLine("Happened");
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(a);
            if (isConnectable(prefs.GetString("reader_ip", "192.168.2.180")))
            {
                TCP tcp = new TCP();
                tcp.Connect(prefs.GetString("reader_ip", "192.168.2.180"), a);
                Stopwatch watch = new Stopwatch();
                watch.Start();

                bool SQLStatus = true;
                try
                {
                    Global.cs.Hello();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Source + " " + e.Message);
                    SQLStatus = false;
                    if (prefs.GetBoolean("visible", true))
                    {
                        a.RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to database!", a));
                    }
                }
                if (SQLStatus)
                {
                    a.RunOnUiThread(() => CreateProgress(a));

                    //Card Stuff
                    string cardSort = prefs.GetString("card_sort", "Name");
                    List<Card> sqlCardList = new List<Card>(Global.cs.GetCardList());
                    sqlCardList = SerializeTools.sortCard(sqlCardList, prefs.GetString("card_sort", "Name"));
                    SerializeTools.serializeCardList(sqlCardList);
                    bar.Progress = 10;

                    //Access Level Stuff
                    string accessSort = prefs.GetString("access_sort", "Name");
                    List<AccessLevel> sqlAccessList = new List<AccessLevel>(Global.cs.GetAccessLevelList());
                    sqlAccessList = SerializeTools.sortAccess(sqlAccessList, prefs.GetString("access_sort", "Name"));
                    SerializeTools.serializeAccessLevelList(sqlAccessList);
                    bar.Progress = 20;

                    //Transaction Stuff
                    List<Transaction> sqlTransList = new List<Transaction>(Global.cs.GetTransactionList());
                    List<Transaction> newList = new List<Transaction>();
                    while (true)
                    {
                        string data = "";
                        tcp.Write("t", out data);
                        if (data.Length < 5)
                            break;
                        if (data.Substring(0, 1) != "t")
                            break;
                        string[] split = data.Split(new char[] { '\t' });
                        int numOfTrans = Convert.ToInt32(split[1]);
                        for (int trans = 0; trans < numOfTrans; trans++)
                        {
                            int offset = trans * 5;
                            int readerNumber = Convert.ToInt32(split[offset + 2]);
                            int cardCode = Convert.ToInt32(split[offset + 3]);

                            long ftime = (((1980L * 365L) + 114) * 86400L);
                            DateTime dateTime = DateTime.MinValue;
                            ftime += Convert.ToInt64(split[offset + 4]);
                            dateTime = dateTime.AddSeconds(ftime);

                            string cardHolder = "Not On File";
                            foreach (Card card in sqlCardList)
                            {
                                if (card.cardCode == cardCode)
                                {
                                    cardHolder = card.name;
                                    break;
                                }
                            }
                            int errorCode = Convert.ToInt32(split[offset + 5]);
                            newList.Add(new Transaction(readerNumber, cardCode, dateTime, errorCode, cardHolder));
                        }
                    }
                    bar.Progress = 50;
                    sqlTransList.AddRange(newList);
                    bool result, resultSpecified;
                    Global.cs.UpdateTransactionSQL(newList.ToArray(), out result, out resultSpecified);
                    bar.Progress = 70;
                    string transactionSort = prefs.GetString("transaction_sort", "Newest to Oldest");
                    sqlTransList = SerializeTools.sortTransaction(sqlTransList, prefs.GetString("transaction_sort", "Newest to Oldest"));
                    bar.Progress = 85;
                    SerializeTools.serializeTransaction(sqlTransList);
                    bar.Progress = 100;
                    Thread.Sleep(500);
                    bar.Dismiss();
                }

                watch.Stop();
                Console.WriteLine("Elapsed miliseconds" + watch.Elapsed.Milliseconds.ToString());
                tcp.Close();
            }
            else
            {
                if (prefs.GetBoolean("visible", true))
                {
                    a.RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to reader!", a));
                }
            }
        }

        public static bool isConnectable(string ip)
        {
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(ip, 500);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source + " " + e.Message);
            }
            return false;
        }


    }
}