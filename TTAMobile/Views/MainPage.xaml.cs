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
        UserInfoModel userInfoModel;
        List<string> userRoles;
        List<string> roleClaims;
        Boolean pageAuthorized;

		public MainPage()
		{
			InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //username.Focus();
        }

        private async void Login_OnClicked(object sender, EventArgs e)
        {
            userInfoModel = new UserInfoModel();
            userInfoModel = await App.RestService.Login(username.Text, password.Text);
            if (userInfoModel != null)
            {
                welcome.Text = "Welcome " + userInfoModel.Username;
                username.Text = "";
                password.Text = "";
                username.IsEnabled = false;
                password.IsEnabled = false;
            }
        }

        private void Logout_OnClicked(object sender, EventArgs e)
        {
            username.IsEnabled = true;
            password.IsEnabled = true;
            welcome.Text = "";
        }

        private async void Page1_OnClicked(object sender, EventArgs e)
        {
            userRoles = await App.RestService.GetUserRoles(userInfoModel.UserId, userInfoModel.Token);

            if (userRoles != null)
            {
                roleClaims = await App.RestService.GetRoleClaims(userRoles, "Page1", userInfoModel.Token); 
            }

            if (roleClaims != null)
            {
                pageAuthorized = false;
                pageAuthorized = App.RestService.CheckPageAuthorization(userRoles, roleClaims);
            }

            if (pageAuthorized)
            {
                await Navigation.PushAsync(new Page1());
            }
            else
            {
                await DisplayAlert("Authorization", "Access Denied", "OK");
            }
        }

        private async void Page2_OnClicked(object sender, EventArgs e)
        {
            userRoles = await App.RestService.GetUserRoles(userInfoModel.UserId, userInfoModel.Token);

            if (userRoles != null)
            {
                roleClaims = await App.RestService.GetRoleClaims(userRoles, "Page2", userInfoModel.Token);
            }

            if (roleClaims != null)
            {
                pageAuthorized = false;
                pageAuthorized = App.RestService.CheckPageAuthorization(userRoles, roleClaims);
            }

            if (pageAuthorized)
            {
                await Navigation.PushAsync(new Page2());
            }
            else
            {
                await DisplayAlert("Authorization", "Access Denied", "OK");
            }
        }
    }
}
