using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Diabetes_Tracker.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BaseMenuItem = Diabetes_Tracker.Models.ListItems.BaseMenuItem;

namespace Diabetes_Tracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseMaster : ContentPage
    {
        public ListView ListView;

        public BaseMaster()
        {
            InitializeComponent();

            BindingContext = new BaseMasterViewModel();
            ListView = MenuItemsListView;
            ProfileName.Text = Session.User.UserName;
        }

        public class BaseMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<BaseMenuItem> MenuItems { get; set; }
            
            public BaseMasterViewModel()
            {
                MenuItems = new ObservableCollection<BaseMenuItem>(new[]
                {
                    new BaseMenuItem { Id = 3, Title = "Dashboard", TargetType = typeof(Dashboard)},
                    new BaseMenuItem { Id = 0, Title = "Logbook", TargetType = typeof(LogbookDates)},
                    //new BaseMenuItem { Id = 1, Title = "Reward Points", TargetType = typeof(RewardPoints)},
                    new BaseMenuItem { Id = 1, Title = "Sign Out",TargetType = typeof(Login), NavigationPage = false}
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}