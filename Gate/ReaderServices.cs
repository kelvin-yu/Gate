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
                foreach (string startTime in level.weekTimeStart) //week start
                {
                    string[] time = startTime.Split(new char[] { ':' });
                    int hours = Convert.ToInt16(time[0]);
                    int minutes = Convert.ToInt16(time[1]);
                    int totalMinutes = hours * 60 + minutes;

                    acc += "\t" + Convert.ToString(totalMinutes);
                }
                foreach (string endTime in level.weekTimeEnd) //week end
                {
                    string[] time = endTime.Split(new char[] { ':' });
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

                    string[] values = level.dateStart.Split(new char[] { '/' });
                    int year = Convert.ToInt16(values[2]);
                    int day = Convert.ToInt16(values[1]);
                    int month = Convert.ToInt16(values[0]);
                    TimeSpan startSpan = TimeSpan.FromTicks(new DateTime(year, month, day).Ticks).Subtract(TimeSpan.FromTicks(new DateTime(1980, 1, 1).Ticks));
                    start = (int)startSpan.Days;
                    values = level.dateEnd.Split(new char[] { '/' });
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
                List<Transaction> transactionList = SerializeTools.deserializeTransactionList();
                Console.WriteLine("Wake");
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
                        string readerNumber = split[offset + 2];
                        string cardCode = split[offset + 3];
                        long dateTime = Convert.ToInt64(split[offset + 4]);
                        string errorCode = split[offset + 5];
                        transactionList.Add(new Transaction(readerNumber, cardCode, dateTime, errorCode));
                    }
                }
                SerializeTools.serializeTransaction(transactionList);
                Thread.Sleep(5000);
            }
        }
    }
}