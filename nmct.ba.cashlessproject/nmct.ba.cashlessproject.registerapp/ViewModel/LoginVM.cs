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
using EidSamples;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using nmct.ba.cashlessproject.model;

namespace nmct.ba.cashlessproject.registerapp.ViewModel
{
    class LoginVM : ObservableObject, IPage
    {
        DispatcherTimer CardReaderTimer;

        public LoginVM()
        {
            CardReaderTimer = new DispatcherTimer();
            CardReaderTimer.Interval = new TimeSpan(0, 0, 2);
            CardReaderTimer.Tick += new EventHandler(ReadCard);

            GetOrganisations();
            CardReaderTimer.Start();
        }

        private void ReadCard(object state, EventArgs e)
        {
            try
            {
                ReadData fnr = new ReadData("beidpkcs11.dll");
                ReadData lnr = new ReadData("beidpkcs11.dll");
                Username = fnr.GetFirstName() + " " + lnr.GetSurname();

                if (Username != " ")
                {
                    CardReaderTimer.Stop();

                    if (CurrentOrganisation != null)
                    {
                        Login();
                    }
                }
            } 
            catch (Exception)
            {

            }
        }

        private Organisation currentOrganisation;
        public Organisation CurrentOrganisation
        {
            get { return currentOrganisation; }
            set
            {
                currentOrganisation = value;
                OnPropertyChanged("CurrentOrganisation");

                if (Username != null && Username != " ")
                {
                    Login();
                }
            }
        }

        private ObservableCollection<Organisation> _Organisations;
        public ObservableCollection<Organisation> Organisations
        {
            get { return _Organisations; }
            set { _Organisations = value; OnPropertyChanged("Organisations"); }
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

        private async void GetOrganisations()
        {
            using (HttpClient Organisation = new HttpClient())
            {
                HttpResponseMessage response = await Organisation.GetAsync("http://localhost:46080/api/Organisation");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Organisations = JsonConvert.DeserializeObject<ObservableCollection<Organisation>>(json);
                }
            }
        }

        private void Login()
        {
            ApplicationVM appvm = App.Current.MainWindow.DataContext as ApplicationVM;
            ApplicationVM.token = GetToken(CurrentOrganisation.ID, Username);

            if (ApplicationVM.token != null)
            {
                appvm.Login(Username);
            }
        }

        private TokenResponse GetToken(int id, string name)
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:46080/token"));
            return client.RequestResourceOwnerPasswordAsync("r" + id.ToString(), name).Result;
        }
    }
}
