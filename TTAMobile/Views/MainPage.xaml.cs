using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TTAMobile
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            username.Focus();
        }

        private async void Login_OnClicked(object sender, EventArgs e)
        {
            var result = await App.RestService.Login(username.Text, password.Text);
            if (result != null)
            {
                welcome.Text = "Welcome " + result.Username;
            }
        }

        private async void Page1_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Page1());
        }

        private async void Page2_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Page2());
        }
    }
}
