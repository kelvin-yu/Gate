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
using Android.Preferences;

namespace Gate
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class Loading : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ReaderServices.UpdateInfo(this);

            Console.WriteLine("Finished Loading");

            StartActivity(typeof(MainActivity));
        }
    }
}