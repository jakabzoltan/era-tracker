using Android.App;
using Android.Content;
using Android.OS;

namespace Diabetes_Tracker.Droid
{
    [Service(Name="Era.Tracker.Bloodsugar.Service",IsolatedProcess = true)]
    public class BloodsugarService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new System.NotImplementedException();
        }
    }
}