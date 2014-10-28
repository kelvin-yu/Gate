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

namespace Gate
{
    public class Transaction
    {
        public string readerNumber { get; set; }
        public string cardCode { get; set; }
        public long dateTime { get; set; }
        public string errorNumber { get; set; }

        public Transaction() { }

        public Transaction(string readerNumber, string cardCode, long dateTime, string errorCode)
        {
            this.readerNumber = readerNumber;
            this.cardCode = cardCode;
            this.dateTime = dateTime;
            this.errorNumber = errorCode;
        }
    }
}