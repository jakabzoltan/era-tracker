using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.GATT;
using Diabetes_Tracker.GATT.CallbackManangers;
using Diabetes_Tracker.GATT.Chracteristics;
using Diabetes_Tracker.GATT.Descriptors;
using Diabetes_Tracker.GATT.Helpers;
using Diabetes_Tracker.GATT.ServiceManagers;
using Diabetes_Tracker.GATT.Services;
using Diabetes_Tracker.Services;
using Java.Interop;
using Java.Util;
using Java.Util.Functions;
using nexus.core;
using nexus.protocols.ble;
using nexus.protocols.ble.scan;
using nexus.protocols.ble.scan.advertisement;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Diabetes_Tracker
{
    public partial class App : Application
    {
        public static GattCallbackService GattCallbackService { get; set; } = new GattCallbackService();
        public static bool NotificationsSetup { get; set; }
        public static BluetoothAdapter Ble { get; private set; }
        public App(BluetoothAdapter ble) : this()
        {
            Ble = ble;

            bluetoothScan();
        }

        private void bluetoothScan()
        {
            if (Ble == null) Ble = BluetoothAdapter.DefaultAdapter;
            Debug.Write("\tStarting BLE analysis");

            Debug.WriteLine("\t=========================================================");
            foreach (var device in Ble.BondedDevices)
            {
                Debug.WriteLine("\t Device Paired -- " + device.Name);
            }
            Debug.WriteLine("\t=========================================================");

            BluetoothDevice accuChek = Ble.BondedDevices.FirstOrDefault(x => x.Name == "Accu-Chek");
            if (accuChek == null)
            {
                Debug.WriteLine("\t No Accu-Chek Found");
                return;
            }


            GattCallbackService.AttachCharacteristicConsumer(new GlucoseCharacteristicConsumer(), new GlucoseContextCharacteristicConsumer());

            var connection = accuChek.ConnectGatt(Android.App.Application.Context, true, GattCallbackService);
            



        }

        public App()
        {
            InitializeComponent();
            var db = new DatabaseContext();
            db.Init();
            if (Session.UserHasSession() == null)
                if (Session.ResumeUserSession() == null)
                {
                    using (var userService = new UserService())
                    {
                        var profs = userService.GetUserProfiles();
                        if (profs.Any())
                        {
                            MainPage = new NavigationPage(new ProfilesPage(profs)) { Title = "Profiles" };
                            return;
                        }
                    }
                    MainPage = new NavigationPage(new Login()) { Title = "Login" };
                    return;
                }
            MainPage = new Base();

        }

        public App(Page startPage)
        {
            InitializeComponent();
            var db = new DatabaseContext();
            db.Init();


            if (Session.UserHasSession() == null)
                if (Session.ResumeUserSession() == null)
                {
                    using (var userService = new UserService())
                    {
                        var profs = userService.GetUserProfiles();
                        if (profs.Any())
                        {
                            MainPage = new NavigationPage(new ProfilesPage(profs)) { Title = "Profiles" };
                            return;
                        }
                    }
                    MainPage = new NavigationPage(new Login()) { Title = "Login" };
                    return;
                }
            MainPage = new Base(startPage);



        }


        protected override void OnStart()
        {
            bluetoothScan();
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {

        }
    }
}
