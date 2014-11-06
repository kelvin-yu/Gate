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
using System.IO;
using System.Threading;

namespace Gate
{
    [Activity(Label = "Settings")]
    public class Settings : Activity
    {
        Button deleteAll, deleteCards, deleteAccessLevels, deleteTransactions;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Settings);

            deleteAll = FindViewById<Button>(Resource.Id.deleteAll);
            deleteCards = FindViewById<Button>(Resource.Id.deleteCards);
            deleteAccessLevels = FindViewById<Button>(Resource.Id.deleteAccessLevels);
            deleteTransactions = FindViewById<Button>(Resource.Id.deleteTransactions);

            deleteAll.Click += delegate 
            { 
                ReaderServices.deleteAll();
                deleteAllXml();
            };

            deleteCards.Click += delegate
            {
                bool result, resultSpecified;
                ReaderServices.deleteAllCards();
                deleteCardXml();
                Global.cs.DeleteAllCards(out result, out resultSpecified);
            };

            deleteAccessLevels.Click += delegate 
            {
                bool result, resultSpecified;
                ReaderServices.deleteAllAccessLevels();
                deleteAccessXml();
                Global.cs.DeleteAllAccess(out result, out resultSpecified);
            };

            deleteTransactions.Click += delegate
            {
                bool result, resultSpecified;
                deleteTransactionXml();
                Global.cs.DeleteAllTransactions(out result, out resultSpecified);
            };

        }

        public void deleteAllXml()
        {
            deleteCardXml();
            deleteAccessXml();
            deleteTransactionXml();
        }

        public void deleteCardXml()
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
            var cardPath = Path.Combine(documentsPath, "card.xml");
            File.Delete(cardPath);
        }

        public void deleteAccessXml()
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
            var accessLevelPath = Path.Combine(documentsPath, "accesslevel.xml");
            File.Delete(accessLevelPath);
        }

        public void deleteTransactionXml()
        {
            string documentsPath = Android.OS.Environment.ExternalStorageDirectory + "/Gate";
            var transactionPath = Path.Combine(documentsPath, "transaction.xml");
            File.Delete(transactionPath);
        }
    }
}