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
using System.Threading;
using Gate.WebReference;

namespace Gate
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class Loading : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            bool SQLStatus = true;
            try
            {
                ReaderServices.UpdateInfo(this);
            }
            catch(Exception e)
            {
                SQLStatus = false;
                Console.WriteLine(e.Message);
            }
            if (SQLStatus)
            {
                Console.WriteLine("Continuing");
                StartActivity(typeof(MainActivity));
            }
        }
    }
}