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

            GetRegisters();
            GetProducts();
        }

        // Customer card reading

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
                    CurrentSale.CustomerID = CurrentCustomer.ID;
                    IsFormEnabled = true;
                }
                else
                {
                    CustomerLabel = "No customer found";
                }

                OnPropertyChanged("CustomerLabel");
            }
        }

        // Sale

        private Sale currentSale;
        public Sale CurrentSale
        {
            get { return currentSale; }
            set
            {
                currentSale = value;
                OnPropertyChanged("CurrentSale");
            }
        }

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
            if (CurrentCustomer.Balance >= (CurrentSale.Amount * CurrentProduct.Price))
            {
                Error = false;
            }
            else
            {
                Error = true;
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
                string json = JsonConvert.SerializeObject(CurrentSale);
                HttpResponseMessage response;
                client.SetBearerToken(ApplicationVM.token.AccessToken);

                response = await client.PostAsync("http://localhost:46080/api/Sale", new StringContent(json, Encoding.UTF8, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    int result = JsonConvert.DeserializeObject<int>(jsonresponse);

                    if (result != 0)
                    {
                        MessageBox.Show("Order processed.");
                        Cancel();
                    }
                }
            }
        }

        // Registers

        private Register currentRegister;
        public Register CurrentRegister
        {
            get { return currentRegister; }
            set
            {
                currentRegister = value;
                OnPropertyChanged("CurrentRegister");
                if (CurrentSale != null)
                {
                    CurrentSale.RegisterID = currentRegister.ID;
                }

                if (!CardReaderTimer.IsEnabled)
                {
                    IsNewEnabled = true;
                }
            }
        }

        private ObservableCollection<Register> _Registers;
        public ObservableCollection<Register> Registers
        {
            get { return _Registers; }
            set { _Registers = value; OnPropertyChanged("Registers"); }
        }

        private async void GetRegisters()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:46080/api/Register");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Registers = JsonConvert.DeserializeObject<ObservableCollection<Register>>(json);
                }
            }
        }

        // Products

        private Product currentProduct;
        public Product CurrentProduct
        {
            get { return currentProduct; }
            set
            {
                currentProduct = value;
                if (CurrentSale != null)
                {
                    CurrentSale.ProductID = currentProduct.ID;
                    CurrentSale.SinglePrice = CurrentProduct.Price;
                    CheckForError();
                }
                OnPropertyChanged("CurrentProduct");
            }
        }

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged("Products"); CurrentProduct = Products[0]; }
        }

        private async void GetProducts()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:46080/api/product");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);
                }
            }
        }

        // Form state handling
        private bool _isFormEnabled = false;
        public bool IsFormEnabled
        {
            get { return _isFormEnabled; }
            set { _isFormEnabled = value; OnPropertyChanged("IsFormEnabled"); }
        }

        private bool _isNewEnabled = false;
        public bool IsNewEnabled
        {
            get { return _isNewEnabled; }
            set { _isNewEnabled = value; OnPropertyChanged("IsNewEnabled"); OnPropertyChanged("IsCancelEnabled"); }
        }

        public bool IsCancelEnabled
        {
            get 
            {
                if (currentRegister == null)
                {
                    return false;
                }

                return !IsNewEnabled; 
            }
        }

        public ICommand NewCommand
        {
            get { return new RelayCommand(New); }
        }

        private void New()
        {
            IsFormEnabled = false;
            IsNewEnabled = false;
            CurrentSale = new Sale() { Amount = 1, RegisterID = CurrentRegister.ID, ProductID = CurrentProduct.ID, SinglePrice = CurrentProduct.Price };
            CardReaderTimer.Start();
        }

        public ICommand CancelCommand
        {
            get { return new RelayCommand(Cancel); }
        }

        private void Cancel()
        {
            CardReaderTimer.Stop();
            CurrentCustomer = null;
            CurrentSale = new Sale() { Amount = 1, RegisterID = CurrentRegister.ID, ProductID = CurrentProduct.ID, SinglePrice = CurrentProduct.Price };
            IsNewEnabled = false;
            IsFormEnabled = false;
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
