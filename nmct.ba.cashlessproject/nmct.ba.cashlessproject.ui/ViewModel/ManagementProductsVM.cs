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

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class ManagementProductsVM : ObservableObject, IPage
    {
        public ManagementProductsVM()
        {
            GetProducts();
        }

        public string Name
        {
            get { return "Products"; }
        }

        private Product currentProduct;
        public Product CurrentProduct
        {
            get { return currentProduct; }
            set { currentProduct = value; SaveMode = true;  OnPropertyChanged("CurrentProduct"); }
        }

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged("Products"); }
        }

        private async void GetProducts()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:46080/api/product");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Products = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);
                }
            }
        }

        public ICommand NewProductCommand
        {
            get { return new RelayCommand(NewProduct); }
        }

        private void NewProduct()
        {
            CurrentProduct = new Product();
            SaveMode = false;
        }

        public ICommand SaveProductCommand
        {
            get { return new RelayCommand(SaveProduct); }
        }

        // false = save, true = update
        private bool SaveMode = false;

        private async void SaveProduct()
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(CurrentProduct);
                HttpResponseMessage response;

                if (SaveMode)
                {
                    response = await client.PutAsync("http://localhost:46080/api/product/" + CurrentProduct.ID.ToString(), new StringContent(json, Encoding.UTF8, "application/json"));
                }
                else
                {
                    response = await client.PostAsync("http://localhost:46080/api/product", new StringContent(json, Encoding.UTF8, "application/json"));
                }

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    int result = JsonConvert.DeserializeObject<int>(jsonresponse);

                    if (result == 1)
                    {
                        GetProducts();
                        CurrentProduct = null;
                    }
                }
            }
        }
    }
}
