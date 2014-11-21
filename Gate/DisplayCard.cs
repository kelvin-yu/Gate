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
    [Activity(Label = "DisplayCard")]
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

        public void InitiateListeners()
        {
            updateButton.Click += delegate
            {
                ISharedPreferences pref = PreferenceManager.GetDefaultSharedPreferences(this);
                if (pref.GetBoolean("working", true))
                    Thread.Sleep(1000);
                if (!nameField.Text.Equals(String.Empty) && !numberField.Text.Equals(String.Empty) && (!cardNameList.Contains(nameField.Text) || nameField.Text.Equals(cardList[cardIndex].name)))
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
                            UpdateCard();
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

        public void UpdateCard()
        {
            string oldName = cardList[cardIndex].name;
            //Keep old date added
            DateTime dateCreated = cardList[cardIndex].dateAdded;
            //Add to local
            cardList.RemoveAt(cardIndex);
            cardList.Add(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), dateCreated));
            SerializeTools.serializeCardList(cardList);
            //to Reader
            ReaderServices.sendCard(cardList);
            //to SQL
            bool result, resultSpecified;
            Global.cs.DeleteCard(oldName, out result, out resultSpecified);
            Global.cs.AddOneCardSQL(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), dateCreated), out result, out resultSpecified);
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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    bool result, resultSpecified;
                    Global.cs.DeleteCard(cardList[cardIndex].name, out result, out resultSpecified);
                    cardList.RemoveAt(cardIndex);
                    SerializeTools.serializeCardList(cardList);
                    ReaderServices.sendCard(cardList);
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}