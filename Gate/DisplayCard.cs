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
using System.Threading;


namespace Gate
{
    [Activity(Label = "Display Card")]
    public class DisplayCard : Activity
    {
        Button cancelButton, updateButton;
        EditText nameField, numberField;
        Spinner spinner;
        TextView dateAdded;

        List<Card> cardList;
        List<AccessLevel> accessLevelList;
        List<string> cardNameList = new List<string>();
        List<string> spinnerList = new List<string>();
        int cardIndex;
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
            bar.SetMessage("Updating Card");
            bar.SetProgressStyle(ProgressDialogStyle.Horizontal);

            foreach (Card card in cardList)
                cardNameList.Add(card.name.ToLower());
        }

        public void InitiateViews()
        {
            cancelButton = FindViewById<Button>(Resource.Id.cancelCardButton);
            updateButton = FindViewById<Button>(Resource.Id.doneCardButton);
            nameField = FindViewById<EditText>(Resource.Id.cardNameField);
            numberField = FindViewById<EditText>(Resource.Id.cardNumberField);
            spinner = FindViewById<Spinner>(Resource.Id.accessLevelSpinner);
            dateAdded = FindViewById<TextView>(Resource.Id.dateAdded);

            updateButton.Text = "Update";

            spinnerList.Add(cardList[cardIndex].accessLevel);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, spinnerList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            spinner.Enabled = false;

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            cardIndex = prefs.GetInt("cardclick", -1);
            Console.WriteLine("Card index is: " + cardIndex);

            nameField.Text = cardList[cardIndex].name;
            numberField.Text = cardList[cardIndex].cardCode.ToString();
            dateAdded.Text = cardList[cardIndex].dateAdded.ToString("MM/dd/yy HH:mm");
        }

        private void AsyncUpdateCard()
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
                        UpdateCard();
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
            updateButton.Click += delegate
            {
                ThreadPool.QueueUserWorkItem(o => AsyncUpdateCard());
            };

            cancelButton.Click += delegate { Finish(); };
        }

        public void UpdateCard()
        {
            string oldName = cardList[cardIndex].name;
            //Keep old date added
            DateTime dateCreated = cardList[cardIndex].dateAdded;
            //Add to local
            cardList.RemoveAt(cardIndex);
            cardList.Add(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), dateCreated));
            SerializeTools.serializeCardList(cardList);
            //to SQL
            bool result, resultSpecified;
            Global.cs.DeleteCard(oldName, out result, out resultSpecified);
            Global.cs.AddOneCardSQL(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), dateCreated), out result, out resultSpecified);
            bar.Progress = 80;
            //to Reader
            ReaderServices.ConnectToReader(this);
            ReaderServices.sendCard(cardList, this);
            bar.Progress = 100;
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu.Add(0, 0, 0, "Delete Card");
            return true;
        }

        private void AsyncDeleteCard()
        {
            bar.Progress = 0;
            RunOnUiThread(() => bar.Show());
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            if (ReaderServices.AreWifiOn(this))
            {
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
                        bool result, resultSpecified;
                        Global.cs.DeleteCard(cardList[cardIndex].name, out result, out resultSpecified);
                        cardList.RemoveAt(cardIndex);
                        SerializeTools.serializeCardList(cardList);
                        ReaderServices.ConnectToReader(this);
                        bar.Progress = 80;
                        ReaderServices.sendCard(cardList, this);
                        bar.Progress = 100;
                        bar.Dismiss();
                        Finish();
                    }
                }
                else
                    RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to reader!"));
            }
            else
            {
                RunOnUiThread(() => CreateOkDialog("No Connection", "Reader or service wifi connections are disabled!"));
                bar.Dismiss();
                return;
            }
            bar.Dismiss();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    ThreadPool.QueueUserWorkItem(o => AsyncDeleteCard());
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}