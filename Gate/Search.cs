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

namespace Gate
{
    [Activity(Label = "Search")]
    public class Search : Activity
    {
        public Spinner typeSpinner, bySpinner;
        public EditText parameter;
        public Button cancel, done;
        public ArrayAdapter byAdapter;
        public TextView date;
        int year = 0, month = 0, day = 0;

        public List<string> type = new List<string>(new string[]{ "Card", "Access Level", "Transaction" });
        public List<string> cardBy = new List<string>(new string[] {"Name", "Card Code", "Access Level"});
        public List<string> accessBy = new List<string>(new string[]{ "Name" });
        public List<string> transactionBy = new List<string>(new string[]{ "Date", "Card Code", "Error Code", "Card Holder" });

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Search);

            InitiateViews();
            InitiateListeners();
        }

        
        private void InitiateViews()
        {
            typeSpinner = FindViewById<Spinner>(Resource.Id.typeSpinner);
            bySpinner = FindViewById<Spinner>(Resource.Id.bySpinner);
            parameter = FindViewById<EditText>(Resource.Id.parameter);
            done = FindViewById<Button>(Resource.Id.searchDone);
            cancel = FindViewById<Button>(Resource.Id.searchCancel);
            date = FindViewById<TextView>(Resource.Id.dateSearch);

            date.Text = DateTime.Now.ToString("M/d/yyyy");
            date.Visibility = ViewStates.Invisible;
            date.Enabled = false;

            var typeAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, type);
            typeAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            typeSpinner.Adapter = typeAdapter;

            byAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, cardBy);
            byAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            bySpinner.Adapter = byAdapter;
        }

        private void InitiateListeners()
        {
            typeSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(type_ItemSelected);
            bySpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(by_ItemSelected);

            done.Click += delegate
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutString("searchtype", typeSpinner.SelectedItem.ToString());
                editor.PutString("searchby", bySpinner.SelectedItem.ToString());
                if (typeSpinner.SelectedItem.ToString().Equals("Transaction") && bySpinner.SelectedItem.ToString().Equals("Date"))
                    editor.PutString("parameter", date.Text);
                else
                    editor.PutString("parameter", parameter.Text);
                editor.Apply();
                Finish();
                StartActivity(typeof(SearchResults));
            };

            date.Click += delegate { ShowDialog(0); };

            cancel.Click += delegate { Finish(); };
        }

        private void DatePickerCallBack(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            date.Text = e.Date.ToString("M/d/yyyy");
        }

        protected override Dialog OnCreateDialog(int id)
        {
            if (id == 0)
            {
                var datePicker = new DatePickerDialog(this, DatePickerCallBack, year, month, day);
                datePicker.UpdateDate(DateTime.Now);
                return datePicker;
            }
            return null;
        }

        private void by_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (typeSpinner.SelectedItemPosition == 2)
            {
                switch (e.Position)
                {
                    case 0:
                        date.Visibility = ViewStates.Visible;
                        date.Enabled = true;
                        parameter.Visibility = ViewStates.Invisible;
                        parameter.Enabled = false;
                        break;
                    default:
                        date.Visibility = ViewStates.Invisible;
                        date.Enabled = false;
                        parameter.Visibility = ViewStates.Visible;
                        parameter.Enabled = true;
                        break;
                }
            }
        }

        private void type_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    byAdapter.Clear();
                    byAdapter.AddAll(cardBy);
                    byAdapter.NotifyDataSetChanged();
                    break;
                case 1:
                    byAdapter.Clear();
                    byAdapter.AddAll(accessBy);
                    byAdapter.NotifyDataSetChanged();
                    break;
                case 2:
                    byAdapter.Clear();
                    byAdapter.AddAll(transactionBy);
                    byAdapter.NotifyDataSetChanged();
                    date.Visibility = ViewStates.Visible;
                    date.Enabled = true;
                    parameter.Visibility = ViewStates.Invisible;
                    parameter.Enabled = false;
                    break;
            }
        }
    }
}