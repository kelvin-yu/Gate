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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Android.Preferences;

using Gate.WebReference;


namespace Gate
{
    public class CardFragment : Fragment
    {
        List<Card> cardList;
        ListView listView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ListDisplay, null);

            listView = view.FindViewById<ListView>(Resource.Id.listView);
            listView.ItemClick += OnListItemClick;

            return view;
        }

        private void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("cardclick", e.Position);
            editor.Apply();

            this.Activity.StartActivity(typeof(DisplayCard));
        }

        public override void OnResume()
        {
            base.OnResume();

            List<string> nameList = new List<string>();

            cardList = SerializeTools.deserializeCardList();

            foreach (Card card in cardList)
            {
                nameList.Add(card.name);
            }
            nameList.Sort();

            var adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, nameList);
            listView.Adapter = adapter;
        }

    }

    public class AccessLevelFragment : Fragment
    {
        List<AccessLevel> accessList;
        ListView listView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ListDisplay, null);

            listView = view.FindViewById<ListView>(Resource.Id.listView);
            listView.ItemClick += OnListItemClick;
            return view;
        }

        private void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("accesslevelclick", e.Position);
            editor.Apply();

            this.Activity.StartActivity(typeof(DisplayAccessLevel));
        }

        public override void OnResume()
        {
            base.OnResume();

            List<string> nameList = new List<string>();

            accessList = SerializeTools.deserializeAccessLevelList();

            Console.WriteLine(accessList.Count);
            foreach (AccessLevel access in accessList)
            {
                nameList.Add(access.name);
            }

            nameList.Sort();

            var adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, nameList);
            listView.Adapter = adapter;
        }
    }

    public class TransactionFragment : Fragment
    {
        List<Transaction> transactionList = new List<Transaction>();
        ListView cardListView, timeListView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ListDisplay2, null);

            cardListView = view.FindViewById<ListView>(Resource.Id.listView1);
            timeListView = view.FindViewById<ListView>(Resource.Id.listView2);

            cardListView.Scroll += delegate
            {
                View v = cardListView.GetChildAt(0);
                int top = (v == null) ? 0 : v.Top;
                timeListView.SetSelectionFromTop(cardListView.FirstVisiblePosition, top);
            };

            timeListView.Scroll += delegate
            {
                View v = timeListView.GetChildAt(0);
                int top = (v == null) ? 0 : v.Top;
                cardListView.SetSelectionFromTop(timeListView.FirstVisiblePosition, top);
            };

            cardListView.ItemClick += OnListItemClick;

            return view;
        }

        
        private void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutInt("transactionclick", e.Position);
            editor.Apply();

            this.Activity.StartActivity(typeof(DisplayTransaction));
        }
         

        public void updateTransactions()
        {
            List<string> cardList = new List<string>();
            List<string> dateList = new List<string>();

            transactionList = SerializeTools.deserializeTransactionList();

            foreach (Transaction transaction in transactionList)
            {
                cardList.Add(transaction.cardCode.ToString());
                dateList.Add(transaction.dateTime.ToString("MM/dd/yy H:mm:ss"));
            }

            cardList.Reverse();
            dateList.Reverse();

            var adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, cardList);
            var adapter2 = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, dateList);
            timeListView.Adapter = adapter2;
            cardListView.Adapter = adapter;
        }


        public override void OnResume()
        {
            base.OnResume();
            updateTransactions();
        }
    }

    public class ReaderFragment : Fragment
    {
        ListView listView;
        string[] readers = { "Reader 1", "Reader 2" };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ListDisplay, null);

            listView = view.FindViewById<ListView>(Resource.Id.listView);

            var adapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, readers);
            listView.Adapter = adapter;

            listView.ItemClick += OnListItemClick;

            return view;
        }

        public void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            string data = "";

            AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle("Open " + readers[e.Position]);
            alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            alertDialog.SetMessage("Open the gate on " + readers[e.Position] + "?");
            alertDialog.SetButton("OK", (s, ev) =>
            {
                if (e.Position == 0)
                    TCP.Write("O1", out data);
                else if (e.Position == 1)
                    TCP.Write("O2", out data);
            });
            alertDialog.SetButton2("Cancel", (s, ev) =>
            {
            });
            alertDialog.Show();
        }
    }
}