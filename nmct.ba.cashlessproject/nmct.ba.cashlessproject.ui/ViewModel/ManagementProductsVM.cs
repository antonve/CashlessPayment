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


        private bool _isDeleteEnabled = false;
        public bool IsDeleteEnabled
        {
            get { return _isDeleteEnabled; }
            set { _isDeleteEnabled = value; OnPropertyChanged("IsDeleteEnabled"); }
        }

        private Product currentProduct;
        public Product CurrentProduct
        {
            get { return currentProduct; }
            set
            { 
                currentProduct = value;
                IsFormEnabled = false;
                if (currentProduct == null)
                {
                    IsEditEnabled = false;
                    IsDeleteEnabled = false;
                }
                else
                {
                    if (SaveMode != true)
                    {
                        IsDeleteEnabled = true;
                    }
                    IsEditEnabled = true;
                }
                SaveMode = false; 
                OnPropertyChanged("CurrentProduct"); 
            }
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
                client.SetBearerToken(ApplicationVM.token.AccessToken);
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
            SaveMode = true;
            IsFormEnabled = true;
            IsEditEnabled = false;
            IsDeleteEnabled = false;
        }

        public ICommand EditProductCommand
        {
            get { return new RelayCommand(EditProduct); }
        }

        private void EditProduct()
        {
            IsFormEnabled = true;
            IsEditEnabled = false;
        }

        public ICommand DeleteProductCommand
        {
            get { return new RelayCommand(DeleteProduct); }
        }

        private async void DeleteProduct()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response;

                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    client.SetBearerToken(ApplicationVM.token.AccessToken);
                    response = await client.DeleteAsync("http://localhost:46080/api/product/" + CurrentProduct.ID.ToString());
                    
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

        public ICommand CancelProductCommand
        {
            get { return new RelayCommand(CancelProduct); }
        }

        private void CancelProduct()
        {
            SaveMode = false;
            CurrentProduct = null;
            IsFormEnabled = false;
        }

        public ICommand SaveProductCommand
        {
            get { return new RelayCommand(SaveProduct); }
        }

        // false = update, true = save
        public bool SaveMode
        {
            get 
            {
                if (CurrentProduct == null)
                {
                    return false;
                }

                return CurrentProduct.isNew; 
            }
            set
            {
                if (CurrentProduct != null) 
                {
                    CurrentProduct.isNew = value; 
                    OnPropertyChanged("SaveMode");
                }
            }
        }

        private async void SaveProduct()
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(CurrentProduct);
                HttpResponseMessage response;
                client.SetBearerToken(ApplicationVM.token.AccessToken);

                if (SaveMode)
                {
                    response = await client.PostAsync("http://localhost:46080/api/product", new StringContent(json, Encoding.UTF8, "application/json"));
                }
                else
                {
                    response = await client.PutAsync("http://localhost:46080/api/product/" + CurrentProduct.ID.ToString(), new StringContent(json, Encoding.UTF8, "application/json"));
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
