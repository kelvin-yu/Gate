using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

using Gate.WebReference;
using Android.Preferences;


namespace Gate
{
    [Activity(Label = "Gate", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public List<Card> Card;
        Fragment[] fragments;
        IMenu menu1;
        Timer timer;

        private void TimerCallback(object state)
        {
            ReaderServices.UpdateInfo(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            SetContentView(Resource.Layout.Main);

            Card = new List<Card>();

            fragments = new Fragment[]
            {
                new CardFragment(),
                new AccessLevelFragment(),
                new TransactionFragment(),
                new ReaderFragment()
            };

            AddTab("Cards");
            AddTab("Access Levels");
            AddTab("Transactions");
            AddTab("Readers");
        }

        protected override void OnResume()
        {
            base.OnResume();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            if (TCP.isTCPNull())
            {
                if (ThreadPool.QueueUserWorkItem(o => TCP.ConnectWithDialog(prefs.GetString("reader_ip", "192.168.2.180"), this)))
                {
                    timer = new Timer(TimerCallback, null, 0, Convert.ToInt32(prefs.GetString("refresh", "1")) * 60000);
                }
            }
            else
            {
                timer = new Timer(TimerCallback, null, 0, Convert.ToInt32(prefs.GetString("refresh", "1")) * 60000);
            }
        }

        public void AddTab(string text)
        {
            ActionBar.Tab tab = ActionBar.NewTab();
            tab.SetText(text);
            tab.TabSelected += TabOnTabSelected;
            ActionBar.AddTab(tab);
        }

        public void TabOnTabSelected(object sender, ActionBar.TabEventArgs tabEventArgs)
        {
            ActionBar.Tab tab = (ActionBar.Tab)sender;
            Console.WriteLine(tab.Position);
            Fragment frag = fragments[tab.Position];
            if (menu1 != null)
            {
                if (tab.Position == 0)
                {
                    menu1.FindItem(1).SetVisible(false);
                    menu1.FindItem(0).SetVisible(true);
                    menu1.FindItem(2).SetVisible(false);
                    menu1.FindItem(4).SetVisible(true);
                }
                else if (tab.Position == 1)
                {
                    menu1.FindItem(1).SetVisible(true);
                    menu1.FindItem(0).SetVisible(false);
                    menu1.FindItem(2).SetVisible(false);
                    menu1.FindItem(4).SetVisible(true);
                }
                else if (tab.Position == 2)
                {
                    menu1.FindItem(0).SetVisible(false);
                    menu1.FindItem(1).SetVisible(false);
                    menu1.FindItem(2).SetVisible(true);
                    menu1.FindItem(4).SetVisible(true);
                }
                else if (tab.Position == 3)
                {
                    menu1.FindItem(0).SetVisible(false);
                    menu1.FindItem(1).SetVisible(false);
                    menu1.FindItem(2).SetVisible(false);
                    menu1.FindItem(4).SetVisible(false);
                }
            }
            tabEventArgs.FragmentTransaction.Replace(Resource.Id.frameLayout1, frag);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            menu1 = menu;
            menu.Add(0, 0, 0, "Add Card");
            menu.Add(1, 1, 1, "Add Access Level");
            menu.Add(2, 2, 2, "Refresh");
            menu.Add(3, 3, 3, "Settings");
            menu.Add(4, 4, 4, "Search");
            menu.GetItem(1).SetVisible(false);
            menu.GetItem(2).SetVisible(false);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case 0:
                    StartActivity(typeof(AddCard));
                    return true;
                case 1:
                    StartActivity(typeof(AddAccessLevel));
                    return true;
                case 2:
                    var fragment = (TransactionFragment)fragments[2];
                    fragment.updateTransactions();
                    return true;
                case 3:
                    StartActivity(typeof(Settings));
                    return true;
                case 4:
                    StartActivity(typeof(Search));
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

    }
}
