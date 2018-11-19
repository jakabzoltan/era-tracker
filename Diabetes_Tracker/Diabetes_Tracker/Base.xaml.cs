using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diabetes_Tracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BaseMenuItem = Diabetes_Tracker.Models.ListItems.BaseMenuItem;

namespace Diabetes_Tracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Base : MasterDetailPage
    {
        public Base()
        {
            try
            {
                InitializeComponent();
                MasterPage.ListView.ItemSelected += ListView_ItemSelected;
                //Title = Detail.Title;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public Base(Page detailPage)
        {
            try
            {
                InitializeComponent();
                MasterPage.ListView.ItemSelected += ListView_ItemSelected;
                Detail = detailPage;
                //Title = Detail.Title;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }


        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as BaseMenuItem;
            if (item == null)
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            if (item.TargetType == typeof(Login))
            {
                Logout();
                return; 
            }



            if (item.NavigationPage)
            {
                Detail = new NavigationPage(page);
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(page);
            }
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }



        public void Logout()
        {
            Session.LogoutUser();
            using (var userService = new UserService())
            {
                var profs = userService.GetUserProfiles();
                if (profs.Any())
                {
                    Application.Current.MainPage = new NavigationPage(new ProfilesPage(profs)) { Title = "Profiles" };
                    return;
                }
            }
            Application.Current.MainPage = new NavigationPage(new Login()) { Title = "Login" };
            MasterPage.ListView.SelectedItem = null;
            return;
        }
    }
}