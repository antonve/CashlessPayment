using GalaSoft.MvvmLight.Command;
using nmct.ba.cashlessproject.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.salesapp.ViewModel
{
    class ApplicationVM : ObservableObject
    {
        public ApplicationVM()
        {
            Pages.Add(new OrderVM());
            // Add other pages

            CurrentPage = Pages[0];
        }

        // PAGE HANDLING
        private IPage currentPage;
        public IPage CurrentPage
        {
            get { return currentPage; }
            set 
            {
                if (currentPage != null)
                {
                    currentPage.IsActive = false;
                }
                currentPage = value;
                if (currentPage != null)
                {
                    currentPage.IsActive = true;
                }
                OnPropertyChanged("CurrentPage");
            }
        }

        private ObservableCollection<IPage> pages;
        public ObservableCollection<IPage> Pages
        {
            get
            {
                if (pages == null) {
                    pages = new ObservableCollection<IPage>();
                }

                return pages;
            }
        }

        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage); }
        }

        private void ChangePage(IPage page)
        {
            CurrentPage = page;
        }

        // AUTHENTICATION
        public static SalesAuth auth = null;

        public void Login()
        {
            Pages.RemoveAt(0);
            Pages.Add(new OrderVM());
            CurrentPage = Pages[0];
            AppTitle = String.Format("Cashless Payment (logged in as {0})", auth.EmployeeName);
        }

        public void Logout()
        {
            while (Pages.Count > 0)
            {
                Pages.RemoveAt(Pages.Count - 1);
            }

            Pages.Add(new LoginVM());
            AppTitle = "Cashless Payment";
            CurrentPage = Pages[0];
            auth = null;
        }

        // ETC
        private string _appTitle = "Cashless Payment";

        public string AppTitle
        {
            get { return _appTitle; }
            set { _appTitle = value; OnPropertyChanged("AppTitle"); }
        }
    }
}
