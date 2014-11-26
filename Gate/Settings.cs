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

namespace Gate
{
    [Activity(Label = "Settings")]
    public class Settings : PreferenceActivity
    {
        Preference deleteAll, deleteCards, deleteAccessLevelsAndCards, deleteTransactions;
        ListPreference cardSort, accessSort, transSort;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AddPreferencesFromResource(Resource.Layout.prefs);

            deleteAll = FindPreference("delete_all");
            deleteCards = FindPreference("delete_all_c");
            deleteAccessLevelsAndCards = FindPreference("delete_all_a_c");
            deleteTransactions = FindPreference("delete_all_t");
            cardSort = (ListPreference) FindPreference("card_sort");
            accessSort = (ListPreference) FindPreference("access_sort");
            transSort = (ListPreference) FindPreference("transaction_sort");

            deleteAll.PreferenceClick += delegate
            {
                bool result, resultSpecified;
                SerializeTools.deleteAllXml();
                ReaderServices.deleteAll(this);
                Global.cs.DeleteAllAccessAndCards(out result, out resultSpecified);
                Global.cs.DeleteAllTransactions(out result, out resultSpecified);
            };

            deleteCards.PreferenceClick += delegate
            {
                bool result, resultSpecified;
                SerializeTools.deleteCardXml();
                ReaderServices.deleteAllCards(this);
                Global.cs.DeleteAllCards(out result, out resultSpecified);
            };

            deleteAccessLevelsAndCards.PreferenceClick += delegate
            {
                bool result, resultSpecified;
                SerializeTools.deleteAccessAndCardXml();
                ReaderServices.deleteAll(this);
                Global.cs.DeleteAllAccessAndCards(out result, out resultSpecified);
            };

            deleteTransactions.PreferenceClick += delegate
            {
                bool result, resultSpecified;
                SerializeTools.deleteTransactionXml();
                Global.cs.DeleteAllTransactions(out result, out resultSpecified);
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
    }
}