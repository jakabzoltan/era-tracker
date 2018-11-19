using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Diabetes_Tracker;
using Debug = System.Diagnostics.Debug;



namespace Diabetes_Tracker.Droid
{
    [Activity(Label = "Diabetes Tracker", Icon = "@drawable/dolphin64", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {



        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var bt = BluetoothAdapter.DefaultAdapter;








            LoadApplication(new App());
        }
    }
}