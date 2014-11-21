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

using Gate.WebReference;

namespace Gate
{
    [Activity(Label = "DisplayTransaction")]
    public class DisplayTransaction : Activity
    {
        TextView holder, card, time, reader, error;
        List<Transaction> transactionList;
        int transactionIndex;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Transaction);

            holder = FindViewById<TextView>(Resource.Id.transactionHolder);
            card = FindViewById<TextView>(Resource.Id.transactionCard);
            time = FindViewById<TextView>(Resource.Id.transactionTime);
            reader = FindViewById<TextView>(Resource.Id.transactionReader);
            error = FindViewById<TextView>(Resource.Id.transactionError);

            transactionList = SerializeTools.deserializeTransactionList();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            transactionIndex = prefs.GetInt("transactionclick", -1);

            holder.Text = transactionList[transactionIndex].cardHolder;
            card.Text = transactionList[transactionIndex].cardCode.ToString();
            time.Text = transactionList[transactionIndex].dateTime.ToString("MM/dd/yy H:mm:ss");

            reader.Text = transactionList[transactionIndex].readerNumber.ToString();
            int errorCode = Convert.ToInt32(transactionList[transactionIndex].errorNumber);
            error.Text = Global.ErrorCode(errorCode);
        }
    }
}