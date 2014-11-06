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


namespace Gate
{
    static class ReaderServices
    {
        static string data = "";

        public static void sendAccessLevel(List<AccessLevel> list)
        {
            deleteAllAccessLevels();
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
                
                TCP.Write(acc, out data);
                Console.WriteLine(data);
            }
        }

        public static void sendCard(List<Card> list)
        {
            deleteAllCards();
            foreach (Card card in list)
            {
                string c = "AC";
                c += "\t" + card.accessLevel;
                c += "\t" + card.cardCode;
                TCP.Write(c, out data);
                Console.WriteLine(c);
            }
        }

        public static void deleteAllAccessLevels()
        {
            Console.WriteLine(TCP.Write("DA", out data));
            Console.WriteLine("deleted all access levels");
        }

        public static void deleteAllCards()
        {
            TCP.Write("DC", out data);
            Console.WriteLine("deleted all cards");
        }

        public static void deleteAll()
        {
            deleteAllAccessLevels();
            deleteAllCards();
        }

        public static void refreshTransactions()
        {
            while(TCP.isConnectable("192.168.2.180"))
            {
                //Card Stuff
                List<Card> sqlCardList = new List<Card>(Global.cs.GetCardList());
                SerializeTools.serializeCardList(sqlCardList);

                //Access Level Stuff
                List<AccessLevel> sqlAccessList = new List<AccessLevel>(Global.cs.GetAccessLevelList());
                SerializeTools.serializeAccessLevelList(sqlAccessList);

                //Transaction Stuff
                List<Transaction> sqlTransList = new List<Transaction>(Global.cs.GetTransactionList());
                List<Transaction> newList = new List<Transaction>();
                while (true)
                {
                    string data = "";
                    TCP.Write("t", out data);
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
                sqlTransList.AddRange(newList);
                bool result, resultSpecified;
                Global.cs.UpdateTransactionSQL(newList.ToArray(), out result, out resultSpecified);
                SerializeTools.serializeTransaction(sqlTransList);
                Thread.Sleep(10000);
            }
        }
    }
}