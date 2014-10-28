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
using Android.Preferences;

namespace Gate
{
    [Activity(Label = "DisplayTransaction")]
    public class DisplayTransaction : Activity
    {
        TextView card, time, reader, error;
        List<Transaction> transactionList;
        int transactionIndex;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Transaction);

            card = FindViewById<TextView>(Resource.Id.transactionCard);
            time = FindViewById<TextView>(Resource.Id.transactionTime);
            reader = FindViewById<TextView>(Resource.Id.transactionReader);
            error = FindViewById<TextView>(Resource.Id.transactionError);

            transactionList = SerializeTools.deserializeTransactionList();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            transactionIndex = prefs.GetInt("cardclick", -1);

            card.Text = transactionList[transactionIndex].cardCode;

            long ftime = (((1980L * 365L) + 114) * 86400L);
            DateTime dateTime = DateTime.MinValue;
            ftime += transactionList[transactionIndex].dateTime;
            dateTime = dateTime.AddSeconds(ftime);
            time.Text = dateTime.ToString("MM/dd/yy H:mm:ss");

            reader.Text = transactionList[transactionIndex].readerNumber;
            int errorCode = Convert.ToInt32(transactionList[transactionIndex].errorNumber);
            if (errorCode == 0)
                error.Text = "No Error";
            else
                error.Text = errorCode.ToString();
        }
    }
}