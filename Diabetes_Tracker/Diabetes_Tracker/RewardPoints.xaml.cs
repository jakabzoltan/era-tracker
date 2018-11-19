using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RewardPoints : ContentPage
	{
		public RewardPoints ()
		{
			InitializeComponent ();
		    Title = "Rewards Points";
		}
	}
}