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
using System.Xml.Serialization;
using System.IO;
using System.Threading;

using Gate.WebReference;
using Android.Preferences;


namespace Gate
{
    [Activity(Label = "Add Card")]
    public class AddCard : Activity
    {
        List<AccessLevel> accessLevelList;
        List<Card> cardList;
        List<string> accessNameList = new List<string>();
        List<string> cardNameList = new List<string>();
        Button cancelButton, doneButton;
        EditText nameField, numberField;
        Spinner spinner;
        TextView dateAdded;
        ProgressDialog bar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddCard);

            accessLevelList = SerializeTools.deserializeAccessLevelList();
            cardList = SerializeTools.deserializeCardList();

            InitiateViews();
            InitiateListeners();

            bar = new ProgressDialog(this);
            bar.SetCancelable(false);
            bar.SetMessage("Sending Card");
            bar.SetProgressStyle(ProgressDialogStyle.Horizontal);
            bar.Max = 100;

            foreach (Card card in cardList)
                cardNameList.Add(card.name.ToLower());
        }

        public void InitiateViews()
        {
            cancelButton = FindViewById<Button>(Resource.Id.cancelCardButton);
            doneButton = FindViewById<Button>(Resource.Id.doneCardButton);
            nameField = FindViewById<EditText>(Resource.Id.cardNameField);
            numberField = FindViewById<EditText>(Resource.Id.cardNumberField);
            dateAdded = FindViewById<TextView>(Resource.Id.dateAdded);

            dateAdded.Text = DateTime.Now.ToString("MM/dd/yy HH:mm");

            for (int i = 0; i < accessLevelList.Count; i++)
                accessNameList.Add(accessLevelList[i].name);

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, accessNameList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner = FindViewById<Spinner>(Resource.Id.accessLevelSpinner);
            spinner.Adapter = adapter;
        }

        private void AsyncAddCard()
        {
            bar.Progress = 0;
            RunOnUiThread(() => bar.Show());
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            if (!nameField.Text.Equals(String.Empty) && !numberField.Text.Equals(String.Empty) && spinner.SelectedItem != null && !cardNameList.Contains(nameField.Text))
            {
                if (!ReaderServices.AreWifiOn(this))
                {
                    RunOnUiThread(() => CreateOkDialog("No Connection", "Reader or service wifi connections are disabled!"));
                    bar.Dismiss();
                    return;
                }
                bar.Progress = 20;
                ReaderServices.ConnectToReader(this);
                bar.Progress = 40;
                if (ReaderServices.isConnectable(prefs.GetString("reader_ip", "192.168.2.180")))
                {
                    bool SQLStatus = true;
                    ReaderServices.ConnectToService(this);
                    try
                    {
                        Global.cs.Hello();
                    }
                    catch
                    {
                        SQLStatus = false;
                        RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to database!"));
                    }
                    if (SQLStatus)
                    {
                        bar.Progress = 60;
                        AddNewCard();
                        bar.Dismiss();
                        Finish();
                    }
                }
                else
                    RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to reader!"));
            }
            else if (nameField.Text.Equals(string.Empty))
                RunOnUiThread(() => CreateOkDialog("Name", "You must set a name!"));
            else if (spinner.SelectedItem == null)
                RunOnUiThread(() => CreateOkDialog("Access Level", "No access levels found!"));
            else if (numberField.Text.Equals(String.Empty))
                RunOnUiThread(() => CreateOkDialog("Card Number", "You must set a card number!"));
            else
                RunOnUiThread(() => CreateOkDialog("Card Name", "Card already exists!"));
            bar.Dismiss();
        }

        public void InitiateListeners()
        {
            doneButton.Click += delegate
            {
                ThreadPool.QueueUserWorkItem(o => AsyncAddCard());
            };
            cancelButton.Click += delegate { Finish(); };
        }

        public void CreateOkDialog(string title, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (s, ev) =>
            {
            });
            alertDialog.Show();
        }

        public void AddNewCard()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            //local
            cardList.Add(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), DateTime.Now));
            cardList = SerializeTools.sortCard(cardList, prefs.GetString("card_sort", "Name"));
            SerializeTools.serializeCardList(cardList);
            //sql
            bool result, resultSpecified;
            Global.cs.AddOneCardSQL(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), DateTime.Now), out result, out resultSpecified);
            bar.Progress = 80;
            //reader
            ReaderServices.ConnectToReader(this);
            ReaderServices.sendCard(cardList, this);
            bar.Progress = 100;
        }
    }
}