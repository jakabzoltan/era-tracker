using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabetes_Tracker.ViewCells
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogbookDateCell : ViewCell
	{
		public LogbookDateCell()
		{
			InitializeComponent ();
		}
	}
}