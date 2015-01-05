using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.registerapp.ViewModel
{
    class ApplicationVM : ObservableObject
    {
        public ApplicationVM()
        {
            Pages.Add(new LoginVM());
            //Pages.Add(new RegisterVM());
            // Add other pages

            CurrentPage = Pages[0];
        }

        // PAGE HANDLING
        private IPage currentPage;
        public IPage CurrentPage
        {
            get { return currentPage; }
            set 
            {
                if (currentPage != null)
                {
                    currentPage.IsActive = false;
                }
                currentPage = value;
                if (currentPage != null)
                {
                    currentPage.IsActive = true;
                }
                OnPropertyChanged("CurrentPage");
            }
        }

        private ObservableCollection<IPage> pages;
        public ObservableCollection<IPage> Pages
        {
            get
            {
                if (pages == null) {
                    pages = new ObservableCollection<IPage>();
                }

                return pages;
            }
        }

        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage); }
        }

        private void ChangePage(IPage page)
        {
            CurrentPage = page;
        }

        // AUTHENTICATION
        public static TokenResponse token = null;
        public Customer auth = null;

        public void Login(string Username)
        {
            GetCustomer(Username);
            Pages.RemoveAt(0);
            Pages.Add(new ManageVM());
            CurrentPage = Pages[0];
        }

        private async void GetCustomer(string username)
        {
            string json = JsonConvert.SerializeObject(username);
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.PostAsync("http://localhost:46080/api/Customer", new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    Customer result = JsonConvert.DeserializeObject<Customer>(jsonresponse);

                    if (result != null)
                    {
                        auth = result;
                        AppTitle = String.Format("Cashless Payment Customer (logged in as {0})", auth.CustomerName);
                        (Pages[0] as ManageVM).CurrentCustomer = auth;
                    }
                }
            }
        }

        public void Logout()
        {
            while (Pages.Count > 0)
            {
                Pages.RemoveAt(Pages.Count - 1);
            }

            Pages.Add(new LoginVM());
            AppTitle = "Cashless Payment Customer";
            CurrentPage = Pages[0];
            auth = null;
        }

        // ETC
        private string _appTitle = "Cashless Payment Customer";

        public string AppTitle
        {
            get { return _appTitle; }
            set { _appTitle = value; OnPropertyChanged("AppTitle"); }
        }
    }
}
