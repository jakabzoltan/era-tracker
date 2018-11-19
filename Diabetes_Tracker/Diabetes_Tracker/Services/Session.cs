using System;
using System.Collections.Generic;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.Models;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace Diabetes_Tracker.Services
{
    public static class Session
    {
        private static readonly string _diabetesUser = "DiabetesUserContext";
        private static readonly string _diabetesProfiles = "DiabetesProfileContext";
        public static Settings AppSettings { get; set; }
        public static UserContext User { get; set; }
        public static object CurrentPage { get; set; }
        public static DatabaseContext db { get; set; }
        public static string FakeUserId = "3afbd7fc-83bc-46b9-956b-8b7cb4e9e244";


        public static void AddUserProfile(UserContext ctx)
        {
            if (Application.Current.Properties.ContainsKey(_diabetesProfiles))
            {
                var list = (List<UserContext>) Application.Current.Properties[_diabetesProfiles];
                list.Add(ctx);
                Application.Current.Properties.Remove(_diabetesProfiles);
                Application.Current.Properties.Add(_diabetesProfiles, list);
            }
            else
            {
                Application.Current.Properties.Add(_diabetesProfiles, new List<UserContext>{ctx});
            }
            Application.Current.SavePropertiesAsync();
        }


        public static UserContext StartSession()
        {
            if (User?.UserId == null) return null;
            User.SessionExpiry = DateTime.Now.AddDays(30);
            App.Current.Properties.Add(_diabetesUser, User);
            App.Current.SavePropertiesAsync();
            return User;
        }

        public static UserContext UserHasSession()
        {
            if (Application.Current.Properties.ContainsKey(_diabetesUser))
            {
                return (UserContext)Application.Current.Properties[_diabetesUser];
            }
            return null;
        }

        public static UserContext ResumeUserSession()
        {
            if (Application.Current.Properties.ContainsKey(_diabetesUser))
            {
                User = (UserContext)Application.Current.Properties[_diabetesUser];
            }


            if (User != null)
            {
                if (User.SessionExpiry <= DateTime.Now)
                    User.SessionExpiry = DateTime.Now.AddDays(30);
            }
            return User;
        }

        public static UserContext LoginUser(string username, string password, bool rememberMe)
        {
            if (username == "Zoltan" && password == "mohawk")
            {
                LoginUserMock();
                if (rememberMe)
                {
                    new UserService().AddUserProfile(User);
                }
                
            }
            return User;
        }

        public static void LogoutUser()
        {
            if (Application.Current.Properties.ContainsKey(_diabetesUser))
            {
                Application.Current.Properties.Remove(_diabetesUser); // remove our session variable
            }
            User = null;
            Application.Current.SavePropertiesAsync();
        }

        private static void LoginUserMock()
        {
            User = new UserContext()
            {
                UserId = FakeUserId,
                AcceptableGlucoseMax = 8.3d,
                AcceptableGlucoseMin = 4.1d,
                NumberOfMeasurements = 5,
                PrimaryContactName = "Mom",
                PrimaryContactPhone = "905-522-9000",
                UserName = "Zoltan",
                FirstName = "Zoltan",
                LastName = "Jakab",
                SessionExpiry = DateTime.Now.AddDays(30),
                Stars = 10,
            };
            StartSession();
            
        }

    }



}