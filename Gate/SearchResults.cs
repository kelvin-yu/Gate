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
    [Activity(Label = "Search Results")]
    public class SearchResults : Activity
    {
        TextView title;
        ListView results;
        string type, by, parameter;
        List<string> resultList = new List<string>();
        List<int> indexList = new List<int>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SearchResults);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            type = prefs.GetString("searchtype", null);
            by = prefs.GetString("searchby", null);
            parameter = prefs.GetString("parameter", null);

            if (type.Equals("Card"))
            {
                List<Card> cardList = SerializeTools.deserializeCardList();
                if (by.Equals("Name"))
                {
                    for (int i = 0; i < cardList.Count; i++)
                    {
                        if (parameter.Equals(cardList[i].name.ToLower()))
                        {
                            resultList.Add(cardList[i].name);
                            indexList.Add(i);
                            break;
                        }
                    }
                }
                else if (by.Equals("Card Code"))
                {
                    for (int i = 0; i < cardList.Count; i++)
                    {
                        if (parameter.Equals(cardList[i].cardCode.ToString()))
                        {
                            resultList.Add(cardList[i].name);
                            indexList.Add(i);
                            break;
                        }
                    }
                }
                else if (by.Equals("Access Level"))
                {
                    for (int i = 0; i < cardList.Count; i++)
                    {
                        if (parameter.Equals(cardList[i].accessLevel.ToLower()))
                        {
                            resultList.Add(cardList[i].name);
                            indexList.Add(i);
                        }
                    }
                }
            }
            else if (type.Equals("Access Level"))
            {
                List<AccessLevel> accessList = SerializeTools.deserializeAccessLevelList();
                if (by.Equals("Name"))
                {
                    for(int i = 0; i < accessList.Count; i++)
                    {
                        if (parameter.Equals(accessList[i].name.ToLower()))
                        {
                            resultList.Add(accessList[i].name);
                            indexList.Add(i);
                            break;
                        }
                    }
                }
            }
            else if (type.Equals("Transaction"))
            {
                List<Transaction> transactionList = SerializeTools.deserializeTransactionList();
                if (by.Equals("Date"))
                {
                    for(int i = 0; i < transactionList.Count; i++)
                    {
                        if(parameter.Equals(transactionList[i].dateTime.ToString("M/d/yyyy")))
                        {
                            resultList.Add(transactionList[i].cardHolder + "  -->   " + transactionList[i].dateTime.ToString("MM/dd/yy H:mm:ss"));
                            indexList.Add(i);
                        }
                    }
                }
                else if (by.Equals("Card Code"))
                {
                    for (int i = 0; i < transactionList.Count; i++)
                    {
                        if (parameter.Equals(transactionList[i].cardCode.ToString()))
                        {
                            resultList.Add(transactionList[i].dateTime.ToString("M/d/yy H:mm:ss"));
                            indexList.Add(i);
                        }
                    }
                }
                else if (by.Equals("Error Code"))
                {
                    for (int i = 0; i < transactionList.Count; i++)
                    {
                        if (parameter.Equals(transactionList[i].errorNumber.ToString()))
                        {
                            resultList.Add(transactionList[i].cardHolder + "  -->   " + transactionList[i].dateTime.ToString("MM/dd/yy H:mm:ss"));
                            indexList.Add(i);
                        }
                    }
                }
                else if (by.Equals("Card Holder"))
                {
                    for (int i = 0; i < transactionList.Count; i++)
                    {
                        if (parameter.Equals(transactionList[i].cardHolder.ToLower()))
                        {
                            resultList.Add(transactionList[i].dateTime.ToString("M/dd/yy H:mm:ss"));
                            indexList.Add(i);
                        }
                    }
                }
            }
            InitiateViews();
            InitateListeners();
        }

        private void InitiateViews()
        {
            title = FindViewById<TextView>(Resource.Id.title);
            results = FindViewById<ListView>(Resource.Id.results);
            title.Text = String.Format("Search for {0} in {1} by {2}", parameter, type, by);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, resultList);
            results.Adapter = adapter;
        }

        private void InitateListeners()
        {
            results.ItemClick += OnListItemClick;
        }

        public void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            if(type.Equals("Card"))
            {
                editor.PutInt("cardclick", indexList[e.Position]);
                editor.Apply();
                Finish();
                StartActivity(typeof(DisplayCard));
            }
            else if (type.Equals("Access Level"))
            {
                editor.PutInt("accesslevelclick", indexList[e.Position]);
                editor.Apply();
                Finish();
                StartActivity(typeof(DisplayAccessLevel));
            }
            else if (type.Equals("Transaction"))
            {
                editor.PutInt("transactionclick", indexList[e.Position]);
                editor.Apply();
                StartActivity(typeof(DisplayTransaction));
            }
        }
    }
}