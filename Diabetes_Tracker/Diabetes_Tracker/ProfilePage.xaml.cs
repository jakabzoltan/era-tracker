using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.Extensions;
using Diabetes_Tracker.Models;
using Diabetes_Tracker.Services;
using Diabetes_Tracker.ViewCells;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilesPage : ContentPage
    {

        public void Setup()
        {
            Profiles.ItemTemplate = new DataTemplate(typeof(ProfileCard));
            LoginPage.Clicked += LoginPage_Clicked;
            Profiles.ItemTapped += (sender, e) =>
            {
                var item = e.Item as UserContext;
                if (item == null) return;
                if (item.SessionExpiry >= DateTime.Now)
                {

                    using (var userService = new UserService())
                    {
                        userService.LoginProfile(item);
                        Session.User = item;
                        Application.Current.MainPage = new Base();
                        return;
                    }
                }

                var loginPage = new Login(item.UserName);
                Navigation.PushAsync(loginPage);
            };
            LoginPage.BackgroundColor = Color.Transparent;
        }


        private void LoginPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Login());
        }

        public ProfilesPage()
        {
            InitializeComponent();
            var prof = new ProfileView();
            BindingContext = prof;
            Profiles.ItemsSource = prof.Profiles;
            Setup();
        }

        public ProfilesPage(ICollection<UserContext> profiles)
        {
            InitializeComponent();
            var prof = new ProfileView(profiles);
            BindingContext = prof;
            Profiles.ItemsSource = profiles;
            Setup();
        }


        public class ProfileView
        {
            public ObservableCollection<UserContext> Profiles { get; set; }
            public ProfileView()
            {
                using (var userService = new UserService())
                {
                    Profiles = userService.GetUserProfiles().ToObservableCollection();
                }
            }

            public ProfileView(ICollection<UserContext> profiles)
            {
                Profiles = profiles.ToObservableCollection();
            }


        }



    }
}