using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.ViewModel
{
    class AccountVM : ObservableObject, IPage
    {
        public AccountVM()
        {
        }

        public string Name
        {
            get { return "Account"; }
        }


        public ICommand ChangePasswordCommand
        {
            get { return new RelayCommand<object>(ChangePassword); }
        }

        private async void ChangePassword(object parameter)
        {
            using (HttpClient client = new HttpClient())
            {
                var values = (object[])parameter;
                Dictionary<string, string> data = new Dictionary<string, string>()
                {
                    {"OldPassword", (values[2] as PasswordBox).Password},
                    {"NewPassword", (values[0] as PasswordBox).Password},
                    {"ConfirmPassword", (values[1] as PasswordBox).Password}
                };
                HttpResponseMessage response;
                client.SetBearerToken(ApplicationVM.token.AccessToken);
                string json = JsonConvert.SerializeObject(data);

                response = await client.PostAsync("http://localhost:46080/api/Account/ChangePassword", new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    int result = JsonConvert.DeserializeObject<int>(jsonresponse);

                    if (result == 1)
                    {
                    }
                }
            }
        }
    }
}
