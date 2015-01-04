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
    class ManagementRegistersVM : ObservableObject, IPage
    {
        public ManagementRegistersVM()
        {
            GetRegisters();
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return "Registers"; }
        }

        private bool _isFormEnabled = false;
        public bool IsFormEnabled
        {
            get { return _isFormEnabled; }
            set { _isFormEnabled = value; OnPropertyChanged("IsFormEnabled"); }
        }

        private Register currentRegister;
        public Register CurrentRegister
        {
            get { return currentRegister; }
            set
            { 
                currentRegister = value;
                OnPropertyChanged("CurrentRegister"); 
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
    }
}
