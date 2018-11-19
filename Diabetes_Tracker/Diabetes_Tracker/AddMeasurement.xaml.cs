using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diabetes_Tracker.Models;
using Diabetes_Tracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMeasurement : ContentPage
    {
        public AddMeasurement()
        {
            InitializeComponent();
            var binding = new LogbookItem();
            BindingContext = binding;
            Submit.Clicked += Submit_Clicked;


            Rating.Title = "How do you feel?";
            Rating.ItemsSource = Feelings.GetAllFeelings().ToList();
            Rating.ItemDisplayBinding = new Binding(nameof(Feeling.Name));
            //Rating.BindingContext = binding.Feeling;

            FoodEaten.AutoSize = EditorAutoSizeOption.TextChanges;
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            try
            {
                var logbookService = new LogbookService();
                var item = (LogbookItem)BindingContext;
                //item.FeelingName = ((Feeling)Rating.SelectedItem).Name;


                //logbookService.AddLogbookItem(item);
                Application.Current.MainPage = new Base();
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}