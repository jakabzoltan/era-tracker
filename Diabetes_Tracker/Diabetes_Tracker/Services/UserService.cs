using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diabetes_Tracker.Database;
using Diabetes_Tracker.Models;

namespace Diabetes_Tracker.Services
{
    public class UserService : IDisposable
    {
        public bool CreateUser(SignupModel model)
        {
            return true;
        }

        public bool AddUserProfile(UserContext user)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    if(db.Context.Table<UserContext>().FirstOrDefault(x=>x.UserId == user.UserId) == null)
                    db.Context.Insert(user, typeof(UserContext));
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            

        }

        public UserContext GetUserProfile(string id)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var user = db.Context.Get<UserContext>(id);
                    user.Logbook = db.Context.Table<LogbookItem>().Where(y => y.UserId == id).ToList();
                    return user;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new UserContext();
            }
        }





        public ICollection<UserContext> GetUserProfiles()
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    return db.Context.Table<UserContext>().ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<UserContext>();
            }
        }

        public void LoginProfile(UserContext ctx)
        {
            if (ctx.UserId == Session.FakeUserId)
                Session.LoginUser("zoltan", "mohawk", true);


        }
        public void LoginUser() { }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
 