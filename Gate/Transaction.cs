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
        public int readerNumber { get; set; }
        public int cardCode { get; set; }
        public DateTime dateTime { get; set; }
        public int errorNumber { get; set; }
        public string cardHolder { get; set; }

        public Transaction() { }

        public Transaction(int readerNumber, int cardCode, DateTime dateTime, int errorCode, string cardHolder)
        {
            this.readerNumber = readerNumber;
            this.cardCode = cardCode;
            this.dateTime = dateTime;
            this.errorNumber = errorCode;
            this.cardHolder = cardHolder;
        }
    }
}