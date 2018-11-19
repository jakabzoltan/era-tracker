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
	public partial class Signup : ContentPage
	{
		public Signup ()
		{
			InitializeComponent ();
            var ctx = new SignupViewModel();

		    BindingContext = ctx;
            Submit.Clicked += Submit_Clicked;
            Cancel.Clicked += Cancel_Clicked;

		}

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(true) ;
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            using (var service = new UserService())
            {
                service.CreateUser((SignupViewModel) BindingContext);
            }
                

        }



	    public class SignupViewModel : SignupModel
	    {
	        public SignupViewModel()
	        {
	            Birthday = DateTime.Now;
	        }
	        public DateTime MaxDate { get; set; } = DateTime.Now;
	        
	    }


    }
}