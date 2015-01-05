using GalaSoft.MvvmLight.CommandWpf;
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
using System.Windows;
using System.Windows.Threading;
using EidSamples;

namespace nmct.ba.cashlessproject.registerapp.ViewModel
{
    class ManageVM : ObservableObject, IPage
    {
        public ManageVM()
        {
        }


        private Customer _customer;
        public Customer CurrentCustomer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged("CurrentCustomer");

                if (_customer != null)
                {
                    CustomerLabel = _customer.CustomerName + " (€" + _customer.Balance.ToString() + ")";
                }
                else
                {
                    CustomerLabel = "No customer found";
                }

                OnPropertyChanged("CustomerLabel");
            }
        }

        // Sale

        private bool _error;
        public bool Error
        {
            get { return _error; }
            set { _error = value; OnPropertyChanged("Error"); }
        }

        public ICommand CheckForErrorCommand
        {
            get { return new RelayCommand(CheckForError); }
        }

        private void CheckForError()
        {
            if (CurrentCustomer.Balance >= 100.0)
            {
                Error = true;
            }
            else
            {
                Error = false;
            }
        }

        public ICommand SaveOrderCommand
        {
            get { return new RelayCommand(Save); }
        }

        private async void Save()
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(new { });
                HttpResponseMessage response;
                client.SetBearerToken(ApplicationVM.token.AccessToken);

                response = await client.PostAsync("http://localhost:46080/api/Sale", new StringContent(json, Encoding.UTF8, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    int result = JsonConvert.DeserializeObject<int>(jsonresponse);

                    if (result != 0)
                    {

                    }
                }
            }
        }

        public string CustomerLabel
        {
            get;
            set;
        }

        // Page handling

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return "Order"; }
        }
    }
}
