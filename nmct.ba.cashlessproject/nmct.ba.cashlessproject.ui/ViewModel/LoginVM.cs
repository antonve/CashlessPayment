using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class LoginVM : ObservableObject, IPage
    {
        public LoginVM()
        {
            Username = "anton";
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return "Login"; }
        }


        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged("Username"); }
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        public ICommand LoginCommand
        {
            get { return new RelayCommand<object>(Login); }
        }

        private void Login(object state)
        {
            string password = (state as PasswordBox).Password;
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            ApplicationVM.token = GetToken(password);

            if (!ApplicationVM.token.IsError)
            {
                Error = null;
                appvm.Login(Username);
            }
            else
            {
                Error = "Login incorrect.";
            }
        }

        private TokenResponse GetToken(string password)
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:46080/token"));
            return client.RequestResourceOwnerPasswordAsync(Username, password).Result;
        }
    }
}
