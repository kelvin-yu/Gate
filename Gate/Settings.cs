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
using System.Threading;
using Android.Net.Wifi;

namespace Gate
{
    [Activity(Label = "Settings")]
    public class Settings : PreferenceActivity
    {
        Preference reader, service;
        Preference deleteAll, deleteCards, deleteAccessLevelsAndCards, deleteTransactions;
        ListPreference cardSort, accessSort, transSort;
        bool getReaderWifi = false, getServiceWifi = false;
        ProgressDialog bar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AddPreferencesFromResource(Resource.Layout.prefs);

            bar = new ProgressDialog(this);
            bar.SetCancelable(false);

            bar.SetProgressStyle(ProgressDialogStyle.Horizontal);

            reader = FindPreference("reader");
            service = FindPreference("service");
            deleteAll = FindPreference("delete_all");
            deleteCards = FindPreference("delete_all_c");
            deleteAccessLevelsAndCards = FindPreference("delete_all_a_c");
            deleteTransactions = FindPreference("delete_all_t");
            cardSort = (ListPreference) FindPreference("card_sort");
            accessSort = (ListPreference) FindPreference("access_sort");
            transSort = (ListPreference) FindPreference("transaction_sort");

            reader.PreferenceClick += delegate
            {
                getReaderWifi = true;
                StartActivity(new Intent(Android.Provider.Settings.ActionWifiSettings));
            };

            service.PreferenceClick += delegate
            {
                getServiceWifi = true;
                StartActivity(new Intent(Android.Provider.Settings.ActionWifiSettings));
            };

            deleteAll.PreferenceClick += delegate
            {
                ThreadPool.QueueUserWorkItem(o => AsyncDeleteAll());
            };

            deleteCards.PreferenceClick += delegate
            {
                ThreadPool.QueueUserWorkItem(o => AsyncDeleteCards());
            };

            deleteAccessLevelsAndCards.PreferenceClick += delegate
            {
                ThreadPool.QueueUserWorkItem(o => AsyncDeleteAccessLevelAndCards());
            };

            deleteTransactions.PreferenceClick += delegate
            {
                ThreadPool.QueueUserWorkItem(o => AsyncDeleteTransactions());
            };

            cardSort.PreferenceChange += CardPreferenceChange;
            accessSort.PreferenceChange += AccessPreferenceChange;
            transSort.PreferenceChange += TransPreferenceChange;
        }
        public void CardPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            SerializeTools.serializeCardList(SerializeTools.sortCard(SerializeTools.deserializeCardList(), (string) e.NewValue));
        }

        public void AccessPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            SerializeTools.serializeAccessLevelList(SerializeTools.sortAccess(SerializeTools.deserializeAccessLevelList(), (string) e.NewValue));
        }

        public void TransPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            SerializeTools.serializeTransaction(SerializeTools.sortTransaction(SerializeTools.deserializeTransactionList(), (string) e.NewValue));
        }

        protected override void OnResume()
        {
            base.OnResume();
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            if (getReaderWifi == true)
            {
                WifiManager wifiManager = (WifiManager)this.GetSystemService(Context.WifiService);
                WifiInfo info = wifiManager.ConnectionInfo;
                if (info.NetworkId != -1)
                {
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutInt("reader_id", info.NetworkId);
                    editor.PutString("reader_ssid", info.SSID.Substring(1, info.SSID.Length - 2));
                    editor.Apply();
                    getReaderWifi = false;
                    CreateOkDialog("Connected", info.SSID + " has become the chosen reader network");
                }
                else
                    CreateOkDialog("No Connection", "Not connected to any network!");
            }
            if (getServiceWifi == true)
            {
                WifiManager wifiManager = (WifiManager)this.GetSystemService(Context.WifiService);
                WifiInfo info = wifiManager.ConnectionInfo;
                if (info.NetworkId != -1)
                {
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutInt("service_id", info.NetworkId);
                    editor.PutString("service_ssid", info.SSID.Substring(1, info.SSID.Length - 2));
                    editor.Apply();
                    getServiceWifi = false;
                    CreateOkDialog("Connected", info.SSID + " has become the chosen service network");
                }
                else
                    CreateOkDialog("No Connection", " Not connected to any network!");
            }

            if (prefs.GetString("reader_ssid", null) != null)
                reader.Summary = prefs.GetString("reader_ssid", null) + " is the set network";
            else
                reader.Summary = "No network set";

            if (prefs.GetString("service_ssid", null) != null)
                service.Summary = prefs.GetString("service_ssid", null) + " is the set network";
            else
                service.Summary = "No network set";
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

        private void AsyncDeleteAll()
        {
            bar.Progress = 0;
            bar.SetMessage("Deleting all");
            RunOnUiThread(() => bar.Show());
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
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
                    bool result, resultSpecified;
                    SerializeTools.deleteAllXml();
                    Global.cs.DeleteAllAccessAndCards(out result, out resultSpecified);
                    Global.cs.DeleteAllTransactions(out result, out resultSpecified);
                    bar.Progress = 80;
                    ReaderServices.ConnectToReader(this);
                    ReaderServices.deleteAll(this);
                    bar.Progress = 100;
                }
            }
            else
                RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to reader!"));
            bar.Dismiss();
        }

        private void AsyncDeleteCards()
        {
            bar.Progress = 0;
            bar.SetMessage("Deleting all cards");
            RunOnUiThread(() => bar.Show());
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
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
                    bool result, resultSpecified;
                    SerializeTools.deleteCardXml();
                    Global.cs.DeleteAllCards(out result, out resultSpecified);
                    bar.Progress = 80;
                    ReaderServices.ConnectToReader(this);
                    ReaderServices.deleteAllCards(this);
                    bar.Progress = 100;
                }
            }
            else
                RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to reader!"));
            bar.Dismiss();
        }

        private void AsyncDeleteAccessLevelAndCards()
        {
            bar.Progress = 0;
            bar.SetMessage("Deleting all access levels and cards");
            RunOnUiThread(() => bar.Show());
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
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
                    bool result, resultSpecified;
                    SerializeTools.deleteAccessAndCardXml();
                    Global.cs.DeleteAllAccessAndCards(out result, out resultSpecified);
                    bar.Progress = 80;
                    ReaderServices.ConnectToReader(this);
                    ReaderServices.deleteAll(this);
                    bar.Progress = 100;
                }
            }
            else
                RunOnUiThread(() => CreateOkDialog("No Connection", "No connection to reader!"));
            bar.Dismiss();
        }

        private void AsyncDeleteTransactions()
        {
            bar.Progress = 0;
            bar.SetMessage("Deleting transaction history");
            RunOnUiThread(() => bar.Show());
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            if (!ReaderServices.AreWifiOn(this))
            {
                RunOnUiThread(() => CreateOkDialog("No Connection", "Reader or service wifi connections are disabled!"));
                bar.Dismiss();
                return;
            }
            bar.Progress = 20;
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
                SerializeTools.deleteTransactionXml();
                Global.cs.DeleteAllTransactions(out result, out resultSpecified);
                bar.Progress = 100;
            }
            bar.Dismiss();
        }
    }
}