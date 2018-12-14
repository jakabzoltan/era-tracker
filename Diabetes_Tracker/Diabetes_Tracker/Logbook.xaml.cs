using System;
using System.Collections.ObjectModel;
using System.Linq;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.Extensions;
using Diabetes_Tracker.Models;
using Diabetes_Tracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Logbook : ContentPage
    {
        private LogbookService LogbookService { get; set; }
        public Logbook(DateTime day)
        {
            LogbookService = new LogbookService();
            InitializeComponent();

            var binding = new LogbookBinding();
            var listItems = LogbookService.GetLogbookItems().Where(x => x.GlucoseMeasurement.BaseTime.Date == day.Date).ToObservableCollection();
            if (!listItems.Any())
            {
                LogbookItems.IsVisible = false;
                NoItems.IsVisible = true;
            }
            else
            {
                LogbookItems.IsVisible = true;
                NoItems.IsVisible = false;
                foreach (var item in listItems)
                {
                    binding.LogbookListItems.Add(new LogbookItemViewModel(item));
                }
            }

            BindingContext = binding;

            LogbookItems.ItemTemplate = new DataTemplate(typeof(ViewCells.LogbookItemCell));
            Title = "Logbook";


            //AddMeasurement.Clicked += AddMeasurement_Clicked;
        }

        private void AddMeasurement_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AddMeasurement() { Title = "Add Measurement" });
        }

        public class LogbookBinding
        {
            public LogbookBinding()
            {
                LogbookListItems = new ObservableCollection<LogbookItemViewModel>();
            }

            public ObservableCollection<LogbookItemViewModel> LogbookListItems { get; set; }
        }

        public class LogbookItemViewModel : LogbookItem
        {
            public LogbookItemViewModel(LogbookItem ctx)
            {
                this.UserId = ctx.UserId;
                this.GlucoseMeasurement = ctx.GlucoseMeasurement;
                this.GlucoseMeasurementContext = ctx.GlucoseMeasurementContext;
            }


            public string StringTimeRecorded => GlucoseMeasurement.BaseTime.ToShortTimeString();
                

            //public string StringFeeling => this.Feeling.Name;

            /// <summary>
            /// Color for the text should it be within a range.
            /// </summary>
            public Color GlucoseTextColor =>
                GlucoseMeasurement.GlucoseConcentration > Session.User.AcceptableGlucoseMin &&
                GlucoseMeasurement.GlucoseConcentration < Session.User.AcceptableGlucoseMax
                        ? Color.Green
                        : Color.Red;

        }
    }
}