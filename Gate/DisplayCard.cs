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
    [Activity(Label = "DisplayCard")]
    public class DisplayCard : Activity
    {
        Button cancelButton, doneButton;
        EditText nameField, numberField;
        Spinner spinner;
        List<Card> cardList;
        List<AccessLevel> accessLevelList;
        List<string> accessNameList = new List<string>();
        int cardIndex;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddCard);

            cancelButton = FindViewById<Button>(Resource.Id.cancelCardButton);
            doneButton = FindViewById<Button>(Resource.Id.doneCardButton);
            nameField = FindViewById<EditText>(Resource.Id.cardNameField);
            numberField = FindViewById<EditText>(Resource.Id.cardNumberField);
            spinner = FindViewById<Spinner>(Resource.Id.accessLevelSpinner);

            accessLevelList = SerializeTools.deserializeAccessLevelList();

            for (int i = 0; i < accessLevelList.Count; i++)
            {
                accessNameList.Add(accessLevelList[i].name);
            }

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, accessNameList);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            cardList = SerializeTools.deserializeCardList();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            cardIndex = prefs.GetInt("cardclick", -1);

            nameField.Text = cardList[cardIndex].name;
            numberField.Text = cardList[cardIndex].cardCode.ToString();

            for(int i = 0; i < accessNameList.Count; i++)
            {
                if (accessNameList[i] == cardList[cardIndex].accessLevel)
                {
                    spinner.SetSelection(i);
                    break;
                }
            }

            spinner.Enabled = false;

            doneButton.Click += delegate
            {
                if (!TCP.isTCPNull() && TCP.isConnected())
                {
                    cardList.RemoveAt(cardIndex);
                    addNewCard(cardList);
                    SerializeTools.serializeCardList(cardList);
                    Finish();
                }
                else
                    Toast.MakeText(this, "No connection to reader", ToastLength.Long);
            };

            cancelButton.Click += delegate { Finish(); };
        }

        public void addNewCard(List<Card> list)
        {
            list.Add(new Card(nameField.Text, Convert.ToInt32(numberField.Text), spinner.SelectedItem.ToString(), DateTime.Now));
            ReaderServices.sendCard(list);
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