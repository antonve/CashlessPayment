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
    class ManagementEmployeesVM : ObservableObject, IPage
    {
        public ManagementEmployeesVM()
        {
            GetEmployees();
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return "Employees"; }
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

        private Employee currentEmployee;
        public Employee CurrentEmployee
        {
            get { return currentEmployee; }
            set
            { 
                currentEmployee = value;
                IsFormEnabled = false;
                if (currentEmployee == null)
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
                OnPropertyChanged("CurrentEmployee"); 
            }
        }

        private ObservableCollection<Employee> _Employees;
        public ObservableCollection<Employee> Employees
        {
            get { return _Employees; }
            set { _Employees = value; OnPropertyChanged("Employees"); }
        }

        private async void GetEmployees()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:46080/api/Employee");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Employees = JsonConvert.DeserializeObject<ObservableCollection<Employee>>(json);
                }
            }
        }

        public ICommand NewEmployeeCommand
        {
            get { return new RelayCommand<object>(NewEmployee); }
        }

        private void NewEmployee(object field)
        {
            CurrentEmployee = new Employee() { EmployeeName = "Employee name" };
            SaveMode = true;
            IsFormEnabled = true;
            IsEditEnabled = false;
            IsDeleteEnabled = false;
            (field as System.Windows.Controls.TextBox).Focus();
            (field as System.Windows.Controls.TextBox).SelectAll();
        }

        public ICommand EditEmployeeCommand
        {
            get { return new RelayCommand(EditEmployee); }
        }

        private void EditEmployee()
        {
            IsFormEnabled = true;
            IsEditEnabled = false;
        }

        public ICommand DeleteEmployeeCommand
        {
            get { return new RelayCommand(DeleteEmployee); }
        }

        private async void DeleteEmployee()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response;

                if (MessageBox.Show("Are you sure you want to delete this Employee?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    client.SetBearerToken(ApplicationVM.token.AccessToken);
                    response = await client.DeleteAsync("http://localhost:46080/api/Employee/" + CurrentEmployee.ID.ToString());
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonresponse = await response.Content.ReadAsStringAsync();
                        int result = JsonConvert.DeserializeObject<int>(jsonresponse);

                        if (result == 1)
                        {
                            GetEmployees();
                            CurrentEmployee = null;
                        }
                    }
                }
            }
        }

        public ICommand CancelEmployeeCommand
        {
            get { return new RelayCommand(CancelEmployee); }
        }

        private void CancelEmployee()
        {
            SaveMode = false;
            CurrentEmployee = null;
            IsFormEnabled = false;
            GetEmployees();
        }

        public ICommand SaveEmployeeCommand
        {
            get { return new RelayCommand(SaveEmployee); }
        }

        // false = update, true = save
        public bool SaveMode
        {
            get 
            {
                if (CurrentEmployee == null)
                {
                    return false;
                }

                return CurrentEmployee.isNew; 
            }
            set
            {
                if (CurrentEmployee != null) 
                {
                    CurrentEmployee.isNew = value; 
                    OnPropertyChanged("SaveMode");
                }
            }
        }

        private async void SaveEmployee()
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(CurrentEmployee);
                HttpResponseMessage response;
                client.SetBearerToken(ApplicationVM.token.AccessToken);

                if (SaveMode)
                {
                    response = await client.PostAsync("http://localhost:46080/api/Employee", new StringContent(json, Encoding.UTF8, "application/json"));
                }
                else
                {
                    response = await client.PutAsync("http://localhost:46080/api/Employee/" + CurrentEmployee.ID.ToString(), new StringContent(json, Encoding.UTF8, "application/json"));
                }

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    int result = JsonConvert.DeserializeObject<int>(jsonresponse);

                    if (result == 1)
                    {
                        SaveMode = false;
                        GetEmployees();
                        CurrentEmployee = null;
                    }
                }
            }
        }
    }
}
