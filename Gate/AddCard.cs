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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddCard);

            cancelButton = FindViewById<Button>(Resource.Id.cancelCardButton);
            doneButton = FindViewById<Button>(Resource.Id.doneCardButton);

            nameField = FindViewById<EditText>(Resource.Id.cardNameField);
            numberField = FindViewById<EditText>(Resource.Id.cardNumberField);

            accessLevelList = SerializeTools.deserializeAccessLevelList();

            for (int i = 0; i < accessLevelList.Count; i++)
            {
                accessNameList.Add(accessLevelList[i].name);
            }

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, accessNameList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner = FindViewById<Spinner>(Resource.Id.accessLevelSpinner);
            spinner.Adapter = adapter;

            cardList = SerializeTools.deserializeCardList();

            foreach (Card card in cardList)
                cardNameList.Add(card.name.ToLower());

            doneButton.Click += delegate
            {
                if (!nameField.Text.Equals(String.Empty) && !numberField.Text.Equals(String.Empty) && spinner.SelectedItem != null && !cardNameList.Contains(nameField.Text))
                {
                    if (!TCP.isTCPNull() & TCP.isConnectable("192.168.2.180"))
                    {
                        addNewCard(cardList);
                        SerializeTools.serializeCardList(cardList);
                        Finish();
                    }
                    else
                    {
                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        AlertDialog alertDialog = builder.Create();
                        alertDialog.SetTitle("Connection Lost");
                        alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                        alertDialog.SetMessage("Connection to reader lost! Attempt to reconnect in settings");
                        alertDialog.SetButton("Cancel", (s, ev) =>
                        {
                        });
                        alertDialog.SetButton2("Reconnect", (s, ev) =>
                        {
                            TCP.CloseTCPClient();
                            ThreadPool.QueueUserWorkItem(o => TCP.Connect("192.168.2.180", this));
                        });
                        alertDialog.Show();
                    }
                }
                else if (nameField.Text.Equals(string.Empty))
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    AlertDialog alertDialog = builder.Create();
                    alertDialog.SetTitle("Name");
                    alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    alertDialog.SetMessage("You must set a name!");
                    alertDialog.SetButton("OK", (s, ev) =>
                    {
                    });
                    alertDialog.Show();
                }
                else if (spinner.SelectedItem == null)
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    AlertDialog alertDialog = builder.Create();
                    alertDialog.SetTitle("Access Level");
                    alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    alertDialog.SetMessage("No access levels found!");
                    alertDialog.SetButton("OK", (s, ev) =>
                    {
                    });
                    alertDialog.Show();
                }
                else if (numberField.Text.Equals(String.Empty))
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    AlertDialog alertDialog = builder.Create();
                    alertDialog.SetTitle("Card Number");
                    alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    alertDialog.SetMessage("You must set a card number!");
                    alertDialog.SetButton("OK", (s, ev) =>
                    {
                    });
                    alertDialog.Show();
                }
                else
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    AlertDialog alertDialog = builder.Create();
                    alertDialog.SetTitle("Card Name");
                    alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                    alertDialog.SetMessage("Card already exists!");
                    alertDialog.SetButton("OK", (s, ev) =>
                    {
                    });
                    alertDialog.Show();
                }
            };

            cancelButton.Click += delegate { Finish(); };
        }

        public void addNewCard(List<Card> list)
        {
            list.Add(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), spinner.SelectedItemPosition));
            ReaderServices.sendCard(list);
        }
    }
}