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
using Microsoft.Win32;
using System.IO;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class ManagementCustomersVM : ObservableObject, IPage
    {
        public ManagementCustomersVM()
        {
            GetCustomers();
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return "Customers"; }
        }

        private bool _isFormEnabled = false;
        public bool IsFormEnabled
        {
            get { return _isFormEnabled; }
            set { _isFormEnabled = value; OnPropertyChanged("IsFormEnabled"); }
        }

        private bool _isEditEnabled = false;
        public bool IsEditEnabled
        {
            get { return _isEditEnabled; }
            set { _isEditEnabled = value; OnPropertyChanged("IsEditEnabled"); }
        }

        private Customer currentCustomer;
        public Customer CurrentCustomer
        {
            get { return currentCustomer; }
            set
            { 
                currentCustomer = value;
                IsFormEnabled = false;
                IsEditEnabled = true;
                OnPropertyChanged("CurrentCustomer"); 
            }
        }

        private ObservableCollection<Customer> _Customers;
        public ObservableCollection<Customer> Customers
        {
            get { return _Customers; }
            set { _Customers = value; OnPropertyChanged("Customers"); }
        }

        private async void GetCustomers()
        {
            using (HttpClient Customer = new HttpClient())
            {
                Customer.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await Customer.GetAsync("http://localhost:46080/api/Customer");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Customers = JsonConvert.DeserializeObject<ObservableCollection<Customer>>(json);
                }
            }
        }

        public ICommand EditCustomerCommand
        {
            get { return new RelayCommand(EditCustomer); }
        }

        private void EditCustomer()
        {
            IsFormEnabled = true;
            IsEditEnabled = false;
        }

        public ICommand CancelCustomerCommand
        {
            get { return new RelayCommand(CancelCustomer); }
        }

        private void CancelCustomer()
        {
            CurrentCustomer = null;
            IsFormEnabled = false;
            GetCustomers();
        }

        public ICommand SaveCustomerCommand
        {
            get { return new RelayCommand(SaveCustomer); }
        }


        private async void SaveCustomer()
        {
            using (HttpClient Customer = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(CurrentCustomer);
                HttpResponseMessage response;
                Customer.SetBearerToken(ApplicationVM.token.AccessToken);
                response = await Customer.PutAsync("http://localhost:46080/api/Customer/" + CurrentCustomer.ID.ToString(), new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    int result = JsonConvert.DeserializeObject<int>(jsonresponse);

                    if (result == 1)
                    {
                        GetCustomers();
                        CurrentCustomer = null;
                    }
                }
            }
        }

        public ICommand AddImageCommand
        {
            get { return new RelayCommand(AddImage); }
        }

        private void AddImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                CurrentCustomer.Picture = GetPhoto(ofd.FileName);
                OnPropertyChanged("CurrentCustomer");
            }
        }

        private byte[] GetPhoto(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            fs.Close();

            return data;
        }

    }
}
