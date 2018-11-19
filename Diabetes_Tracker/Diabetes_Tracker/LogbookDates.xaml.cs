using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diabetes_Tracker.Extensions;
using Diabetes_Tracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogbookDates : ContentPage
	{
	    private LogbookService LogbookService { get; set; }
        public LogbookDates ()
		{
	        LogbookService = new LogbookService();
	        InitializeComponent();

	        var binding = new LogbookBinding();
		    var listItems = LogbookService.GetLogbookItems();
            var dates = listItems.Select(x => x.GlucoseMeasurement.BaseTime).DistinctBy(x => x.ToShortDateString()).OrderByDescending(x => x.Ticks).ToObservableCollection();
            if (!listItems.Any())
	        {
	            LogbookItems.IsVisible = false;
	            NoItems.IsVisible = true;
	        }
	        else
	        {
	            LogbookItems.IsVisible = true;
	            NoItems.IsVisible = false;

	            foreach (var item in dates)
	            {
	                var num = listItems.Count(x => x.GlucoseMeasurement.BaseTime.ToShortDateString() == item.ToShortDateString());

                    var stars = (int)Math.Round((((decimal)num / (decimal)Session.User.NumberOfMeasurements) * 3));

                    binding.LogbookListItems.Add(new LogbookDateViewModel(item, stars));
	            }

	            LogbookItems.ItemTapped += (sender, e) =>
	            {
	                var item = ((LogbookDateViewModel)e.Item);

	                Navigation.PushAsync(new Logbook(item.Date) { Title = item.DateString });
	            };

            }

            BindingContext = binding;

	        LogbookItems.ItemTemplate = new DataTemplate(typeof(ViewCells.LogbookDateCell));
	        Title = "Logbook";


	        AddMeasurement.Clicked += AddMeasurement_Clicked;
	    }

	    private void AddMeasurement_Clicked(object sender, System.EventArgs e)
	    {
	        Navigation.PushAsync(new AddMeasurement() { Title = "Add Measurement" });
	    }

	    public class LogbookBinding
	    {
	        public LogbookBinding()
	        {
	            LogbookListItems = new ObservableCollection<LogbookDateViewModel>();
	        }

	        public ObservableCollection<LogbookDateViewModel> LogbookListItems { get; set; }
	    }

        public class LogbookDateViewModel
	    {
	        public LogbookDateViewModel(DateTime date, int stars)
	        {
	            Date = date;
	            Stars = "";
	            for (int i = 0; i < stars; i++)
	            {
	                Stars += " ★";
	            }
                
	        }
            public DateTime Date { get; set; }
	        public string DateString => Date.ToLongDateString();
            public string Stars { get; set; }
	    }
    }
}