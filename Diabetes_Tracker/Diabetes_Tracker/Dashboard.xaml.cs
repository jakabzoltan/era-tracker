using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.Services;
using nexus.protocols.ble;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Dashboard : ContentPage
	{
		public Dashboard ()
		{
			InitializeComponent ();
            
            var logbookService = new LogbookService();
            var model = new DashboardView();

		    this.DeleteDatabase.Clicked += (sender, args) =>
		    {
		        var db = new DatabaseContext().WipeDb(true);
		    };

		    BindingContext = model;
		}
	}

    internal class DashboardView
    {
        public int MeasurementsLeftForTheDay { get; set; }
        public int StarsEarnedToday { get; set; }
        public DateTime NextMeasurementTime { get; set; }

    }




}