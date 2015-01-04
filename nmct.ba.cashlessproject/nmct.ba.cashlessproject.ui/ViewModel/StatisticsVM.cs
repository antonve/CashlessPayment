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

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class StatisticsVM : ObservableObject, IPage
    {
        public StatisticsVM()
        {
            GetStatistics();
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return "Statistics"; }
        }

        private bool _filterDate;
        public bool FilterDate
        {
            get { return _filterDate; }
            set { _filterDate = value; OnPropertyChanged("FilterDate"); }
        }

        private bool _filterRegister;
        public bool FilterRegister
        {
            get { return _filterRegister; }
            set { _filterRegister = value; OnPropertyChanged("FilterRegister"); }
        }

        private bool _filterProduct;
        public bool FilterProduct
        {
            get { return _filterProduct; }
            set { _filterProduct = value; OnPropertyChanged("FilterProduct"); }
        }

        private Nullable<DateTime> _filterDateFrom;
        public Nullable<DateTime> FilterDateFrom
        {
            get { return _filterDateFrom; }
            set { _filterDateFrom = value; OnPropertyChanged("FilterDateFrom"); Filter(); }
        }

        private Nullable<DateTime> _filterDateUntil;
        public Nullable<DateTime> FilterDateUntil
        {
            get { return _filterDateUntil; }
            set { _filterDateUntil = value; OnPropertyChanged("FilterDateUntil"); Filter(); }
        }

        private ObservableCollection<Register> _registers;
        public ObservableCollection<Register> Registers
        {
            get { return _registers; }
            set { _registers = value; OnPropertyChanged("Registers"); }
        }

        private Register _selectedRegister;
        public Register SelectedRegister
        {
            get { return _selectedRegister; }
            set { _selectedRegister = value; OnPropertyChanged("SelectedRegister"); Filter(); }
        }

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged("Products"); }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set { _selectedProduct = value; OnPropertyChanged("SelectedProduct"); Filter(); }
        }

        public string TotalEarned
        {
            get
            {
                double total = 0;

                if (CurrentStatistics == null)
                {
                    return total.ToString();
                }

                foreach (Sale sale in CurrentStatistics)
                {
                    total += sale.TotalPrice;
                }

                return total.ToString() + " euro";
            }
        }

        public string TotalProductsSold
        {
            get
            {
                int total = 0;

                if (CurrentStatistics == null)
                {
                    return total.ToString();
                }

                foreach (Sale sale in CurrentStatistics)
                {
                    total += sale.Amount;
                }

                return total.ToString();
            }
        }

        private ObservableCollection<Sale> currentStatistics;
        public ObservableCollection<Sale> CurrentStatistics
        {
            get { return currentStatistics; }
            set
            { 
                currentStatistics = value;
                OnPropertyChanged("CurrentStatistics");
                OnPropertyChanged("TotalProductsSold");
                OnPropertyChanged("TotalEarned");
            }
        }

        private ObservableCollection<Sale> _Statistics;
        public ObservableCollection<Sale> Statistics
        {
            get { return _Statistics; }
            set { _Statistics = value; OnPropertyChanged("Statistics"); }
        }

        private async void GetStatistics()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:46080/api/Sale");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Statistics = JsonConvert.DeserializeObject<ObservableCollection<Sale>>(json);
                    CurrentStatistics = Statistics;
                    Registers = new ObservableCollection<Register>();
                    Products = new ObservableCollection<Product>();

                    foreach (Sale sale in Statistics)
                    {
                        if (!Registers.Any(currentElement => currentElement.ID == sale.RegisterID))
                        {
                            Registers.Add(new Register() { ID = sale.RegisterID, RegisterName = sale.RegisterName });
                        }
                        if (!Products.Any(currentElement => currentElement.ID == sale.ProductID))
                        {
                            Products.Add(new Product() { ID = sale.ProductID, ProductName = sale.ProductName });
                        }
                    }
                }
            }
        }

        public ICommand FilterCommand
        {
            get { return new RelayCommand(Filter); }
        }

        private void Filter()
        {
            ObservableCollection<Sale> temp = new ObservableCollection<Sale>(Statistics);

            if (FilterDate && FilterDateFrom != null && FilterDateUntil != null)
            {
                for (int i = temp.Count - 1; i >= 0; i--)
                {
                    if (!(temp[i].Timestamp >= FilterDateFrom && temp[i].Timestamp <= FilterDateUntil))
                    {
                        temp.RemoveAt(i);
                    }
                }
            }

            if (FilterRegister && SelectedRegister != null)
            {
                for (int i = temp.Count - 1; i >= 0; i--)
                {
                    if (temp[i].RegisterID != SelectedRegister.ID)
                    {
                        temp.RemoveAt(i);
                    }
                }
            }

            if (FilterProduct && SelectedProduct != null)
            {
                for (int i = temp.Count - 1; i >= 0; i--)
                {
                    if (temp[i].ProductID != SelectedProduct.ID)
                    {
                        temp.RemoveAt(i);
                    }
                }
            }

            CurrentStatistics = temp;
        }
    }
}
