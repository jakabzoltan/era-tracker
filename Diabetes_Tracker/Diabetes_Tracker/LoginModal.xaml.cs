using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diabetes_Tracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentPage
	{
		public Login ()
		{
            //if(ApplicationSessionManager.UserHasSession() != null)
		    //ApplicationSessionManager.LogoutUser();

			InitializeComponent();
		    LoginButton.Clicked += LoginAction;
            SignUpButton.Clicked += SignUpButton_Clicked;
		    Username.Completed += delegate { Password.Focus(); };
		    Password.Completed += LoginAction;
		    Title = "Login";
		}

	    public Login(string username) : this()
	    {
	        Username.Text = username;
	    }

        private void SignUpButton_Clicked(object sender, EventArgs e)
        {
            var nav = new Signup() { Title = "Sign Up" };
            Navigation.PushAsync(nav);
        }

        private void LoginAction(object sender, EventArgs e)
	    {
	        var user = Session.LoginUser(Username.Text, Password.Text, RememberMe.IsToggled);

	        if (user == null)
	        {
	            DisplayAlert("Unathorized", "Your credentials could not be validated. Try again.", "OK");
	        }
	        else
	        {
	            Application.Current.MainPage = new Base();
	        }

	    }
    }


}