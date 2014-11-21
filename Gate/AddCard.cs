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
    [Activity(Label = "AddCard")]
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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddCard);

            accessLevelList = SerializeTools.deserializeAccessLevelList();
            cardList = SerializeTools.deserializeCardList();

            InitiateViews();
            InitiateListeners();

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

        public void InitiateListeners()
        {
            doneButton.Click += delegate
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                if (prefs.GetBoolean("working", true))
                    Thread.Sleep(1000);
                if (!nameField.Text.Equals(String.Empty) && !numberField.Text.Equals(String.Empty) && spinner.SelectedItem != null && !cardNameList.Contains(nameField.Text))
                {
                    if (!TCP.isTCPNull() & TCP.isConnectable(TCP.ip))
                    {
                        bool SQLStatus = true;
                        try
                        {
                            Global.cs.Hello();
                        }
                        catch
                        {
                            SQLStatus = false;
                            CreateOkDialog("No Connection", "No connection to database!");
                        }
                        if (SQLStatus)
                        {
                            AddNewCard();
                            Finish();
                        }
                    }
                    else
                        CreateOkDialog("No Connection", "No connection to reader!");
                }
                else if (nameField.Text.Equals(string.Empty))
                    CreateOkDialog("Name", "You must set a name!");
                else if (spinner.SelectedItem == null)
                    CreateOkDialog("Access Level", "No access levels found!");
                else if (numberField.Text.Equals(String.Empty))
                    CreateOkDialog("Card Number", "You must set a card number!");
                else
                    CreateOkDialog("Card Name", "Card already exists!");
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
            //reader
            ReaderServices.sendCard(cardList);
            //sql
            bool result, resultSpecified;
            Global.cs.AddOneCardSQL(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), DateTime.Now), out result, out resultSpecified);
        }
    }
}