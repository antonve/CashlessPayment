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

namespace nmct.ba.cashlessproject.salesapp.ViewModel
{
    class OrderVM : ObservableObject, IPage
    {
        DispatcherTimer CardReaderTimer;

        public OrderVM()
        {
            ApplicationVM.auth = new SalesAuth() { OrganisationID = 2015 };
            CurrentCustomer = null;
            CardReaderTimer = new DispatcherTimer();
            CardReaderTimer.Interval = new TimeSpan(0, 0, 2);
            CardReaderTimer.Tick += new EventHandler(ReadCard);

            CardReaderTimer.Start();
        }

        private void ReadCard(object state, EventArgs e)
        {
            try
            {
                ReadData fnr = new ReadData("beidpkcs11.dll");
                ReadData lnr = new ReadData("beidpkcs11.dll");
                string username = fnr.GetFirstName() + " " + lnr.GetSurname();

                if (username != " ")
                {
                    CardReaderTimer.Stop();

                    GetCustomer(username);
                }
            }
            catch (Exception)
            {

            }
        }

        public string CustomerLabel
        {
            get;
            set;
        }

        private async void GetCustomer(string username)
        {
            string json = JsonConvert.SerializeObject(username);
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync("http://localhost:46080/api/Customer?org=" + ApplicationVM.auth.OrganisationID, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    Customer result = JsonConvert.DeserializeObject<Customer>(jsonresponse);

                    if (result != null)
                    {
                        CurrentCustomer = result;
                    }
                    else
                    {
                        CardReaderTimer.Start();
                    }
                }
            }
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
                    OnPropertyChanged("CustomerLabel");
                }
                else
                {
                    CustomerLabel = "No customer found";
                }
            }
        }

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
