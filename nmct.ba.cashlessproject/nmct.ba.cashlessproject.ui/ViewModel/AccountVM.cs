using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class AccountVM : ObservableObject, IPage
    {
        public AccountVM()
        {
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return "Account"; }
        }

        private string _error;
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                OnPropertyChanged("Error");
            }
        }

        public ICommand LogoutCommand
        {
            get { return new RelayCommand(Logout); }
        }

        private void Logout()
        {
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            appvm.Logout();
        }

        public ICommand ChangePasswordCommand
        {
            get { return new RelayCommand<object>(ChangePassword); }
        }

        private async void ChangePassword(object parameter)
        {
            using (HttpClient client = new HttpClient())
            {
                var values = (object[])parameter;
                Dictionary<string, string> data = new Dictionary<string, string>()
                {
                    {"OldPassword", (values[2] as PasswordBox).Password},
                    {"NewPassword", (values[0] as PasswordBox).Password},
                    {"ConfirmPassword", (values[1] as PasswordBox).Password}
                };
                HttpResponseMessage response;
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string json = JsonConvert.SerializeObject(data);

                response = await client.PutAsync("http://localhost:46080/api/Account/ChangePassword", new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Password changed, please log in again.");
                    Logout();
                }
                else
                {
                    Error = response.ReasonPhrase;
                }
            }
        }
    }
}
